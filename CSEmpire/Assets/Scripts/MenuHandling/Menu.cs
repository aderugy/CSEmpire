using UnityEngine;

namespace MenuHandling
{
    /// <author>Arthur de Rugy</author>
    /// <summary>
    /// Implements basic operations on different menus.
    /// </summary>
    public class Menu : MonoBehaviour
    {
        /// <summary>
        /// Name of the menu.
        /// </summary>
        public string menuName;

        private bool _open = true;
        
        /// <summary>
        /// Opens the current menu.
        /// It basically makes it visible.
        /// </summary>
        public void Open()
        {
            gameObject.SetActive(true);
            _open = true;
        }

        /// <summary>
        /// Opens the current menu.
        /// It basically makes it not visible.
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
            _open = false;
        }
    }
}
