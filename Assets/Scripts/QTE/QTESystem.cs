using UnityEngine.InputSystem;
using System.Collections.Generic;
using Mekaiju.AI.PhaseAttack;
using System.Collections;
using UnityEngine;
namespace Mekaiju.QTE
{
    public class QTESystem
    {
        private MechaPlayerActions _qteAction;

        public List<InputAction> qteInputActions { get; private set; }

        float _currentInput;
        PhaseAttack _phaseAttack;
        bool _qteActive;

        ShowQTE _showQTE;

        public QTESystem(PhaseAttack p_phaseAttack)
        {
            _qteActive = false;
            qteInputActions = new List<InputAction>();
            _qteAction = new MechaPlayerActions();
            _phaseAttack = p_phaseAttack;

            foreach (var action in _qteAction)
            {
                if (action.actionMap.name == "QTE")
                {
                    qteInputActions.Add(action);
                }
            }
            _currentInput = 0;

            _qteAction.Enable();
            _qteAction.QTE.Q.performed += (context) => QTEPressed(context, 0);
            _qteAction.QTE.E.performed += (context) => QTEPressed(context, 1);
            _qteAction.QTE.Space.performed += (context) => QTEPressed(context, 2);

            _showQTE = ShowQTE.instance;
            _showQTE.SetForeground(0);
            _showQTE.SetOutline(0);
            _showQTE.qteUI.SetActive(false);
        }

        public void QTEPressed(InputAction.CallbackContext p_context, int p_input)
        {
            if (!_qteActive) return;
            if (_phaseAttack.input == p_input)
            {
                _currentInput++;
            }
        }

        public void StartQTE()
        {
            _currentInput = 0;
            _qteActive = true;

            _showQTE.qteUI.SetActive(true);

            _showQTE.StartCoroutine(qteSpam());

            _showQTE.SetName(qteInputActions[_phaseAttack.input].GetBindingDisplayString(0));
        }

        public IEnumerator qteSpam()
        {
            float t_time = 0;
            bool t_finish = false;
            while (!t_finish)
            {
                 yield return new WaitForSeconds(.1f);
                t_time += .1f;

                _currentInput -= .1f;
                if(_currentInput < 0)
                {
                    _currentInput = 0;
                }
                _showQTE.SetOutline(t_time / _phaseAttack.time);
                _showQTE.SetForeground(_currentInput / _phaseAttack.quantity);

                if(_currentInput >= _phaseAttack.quantity)
                {
                    t_finish = true;
                }

                if (t_time >= _phaseAttack.time)
                {
                    t_finish = true;
                }
            }

            if (_currentInput >= _phaseAttack.quantity)
            {
                _phaseAttack.Success();
            }
            else
            {
                _phaseAttack.Failure();
            }
            _showQTE.qteUI.SetActive(false);
            _qteActive = false;
        }
    }
}
