using RPGM.Core;
using RPGM.Gameplay;
using UnityEngine;

namespace RPGM.UI
{
    /// <summary>
    /// Sends user input to the correct control systems.
    /// </summary>
    public class InputController : MonoBehaviour
    {
        public float stepSize = 0.1f;
        GameModel model = Schedule.GetModel<GameModel>();

        public enum State
        {
            CharacterControl,
            DialogControl,
            Pause
        }

        State state;

        public void ChangeState(State state) => this.state = state;

        void Update()
        {
            switch (state)
            {
                case State.CharacterControl:
                    CharacterControl();
                    break;
                case State.DialogControl:
                    DialogControl();
                    break;
            }
        }

        void DialogControl()
        {
            model.player.nextMoveCommand = Vector3.zero;
            int horizontalClamp = (int)Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);
            if (Mathf.Abs(horizontalClamp) > 0)
                model.dialog.FocusButton(horizontalClamp);
            if (Input.GetButtonDown("Fire1"))
                model.dialog.SelectActiveButton();
        }

        void CharacterControl()
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");
            Vector3 movementCommand = new Vector3(horizontalAxis*stepSize, verticalAxis*stepSize, 0);
            model.player.nextMoveCommand = movementCommand;
        }
    }
}