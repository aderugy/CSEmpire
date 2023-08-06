const express = require('express');
const mysql = require('mysql2');
const body_parser = require('body-parser');
require('dotenv').config()

const app = express();
app.use(body_parser.json());

const connection = mysql.createConnection({
  host: process.env.HOST,
  user: process.env.USERNAME,
  password: process.env.PASSWORD,
  database: process.env.DATABASE
})

const { promisify } = require('util');
const query = promisify(connection.query).bind(connection);

app.use((req, res, next) => {
    res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content, Accept, Content-Type, Authorization');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, PATCH, OPTIONS');
    next();
});

app.post('/api/v1/register', async (req, res) => {
    try {
        const { username, password, email } = req.body;

        if (!username || !password || !email)
            return res.status(400).send();

        const result = await query('SELECT username FROM users WHERE username=?;', [username]);

        if (result.length !== 0)
            return res.status(409).send();

        await query('INSERT INTO users(username, password, email) VALUES(?, ?, ?);', [username, password, email]);
        return res.status(200).send();
    }
    catch (err) {
        console.error(err);
        return res.status(500).send();
    }
})

app.get('/api/v1/login', async (req, res) => {
    try {
        const { username, password } = req.query;

        if (!username || !password)
            return res.status(400).send();

        const result = await query('SELECT password FROM users WHERE username=?;', [username])

        if (result.length === 0 || password !== result[0].password)
            return res.status(401).send();

        return res.status(200).send();
    }
    catch (err) {
        return res.status(500).send();
    }
})

const openedRooms = [];

app.get('/api/v1/room', (req, res) => {
    const username = req.query.username;

    if (!username)
        return res.status(400).send();

    let room;

    if (openedRooms.length === 0) {
        openedRooms.push(username);
        room = username;
    }
    else
        room = openedRooms.shift();

     return res.status(200).json({ room: room }).send();
})


module.exports = app;