using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Player
{
    /// <author>Arthur de Rugy</author>
    /// <summary>
    /// This class's purpose is to define all the bindings that we will use when processing inputs from the user.
    /// These inputs have to be loaded before accessing them.
    /// </summary>
    public static class Inputs
    {
        // MOVEMENTS
        public static KeyCode MoveForwardKey;
        public static KeyCode MoveBackwardsKey;
        public static KeyCode MoveRightKey;
        public static KeyCode MoveLeftKey;
        public static KeyCode JumpKey;
        public static KeyCode WalkKey;


        // MOUSE INPUTS
        public static MouseButton ShootButton;
        public static MouseButton AimButton;

        /// <author>Arthur de Rugy</author>
        /// <summary>
        /// This method will initialize all of the defined bindings.
        /// Why use a method and not initialize the fields directly ?
        /// This way, we can different ways of loading the bindings.
        /// For example, we will likely load them from a file in the future, or from a server.
        /// Nothing changes in the code, you only have to access the fields.
        /// </summary>
        public static void LoadBindings()
        {
            MoveForwardKey = KeyCode.W;
            MoveBackwardsKey = KeyCode.S;
            MoveRightKey = KeyCode.D;
            MoveLeftKey = KeyCode.A;
            JumpKey = KeyCode.Space;
            WalkKey = KeyCode.LeftShift;
            
            ShootButton = MouseButton.LeftMouse;
            AimButton = MouseButton.RightMouse;
        }
    }
}
