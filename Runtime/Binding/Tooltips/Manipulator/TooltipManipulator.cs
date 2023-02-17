using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MVVMToolkit.Settings;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVVMToolkit.Binding.Tooltips
{
    public class TooltipManipulator : Manipulator, IDisposable
    {
        private readonly TooltipElement _tooltip;

        public TooltipManipulator()
        {
            _tooltip = new();
            _tooltip.RegisterCallback<PointerEnterEvent>(OnTooltipEnter);
            _tooltip.RegisterCallback<PointerLeaveEvent>(OnTooltipLeave);

            _cts = new();
        }

        public void Dispose()
        {
            _currentParent?.Remove(_tooltip);
            _tooltip.UnregisterCallback<PointerEnterEvent>(OnTooltipEnter);
            _tooltip.UnregisterCallback<PointerLeaveEvent>(OnTooltipLeave);
        }

        private bool _tooltipHover;

        private void OnTooltipEnter(PointerEnterEvent evt)
        {
            _tooltipHover = true;
        }

        private void OnTooltipLeave(PointerLeaveEvent evt)
        {
            _tooltipHover = false;

            if (!_bindingHover)
                HideTooltip();
        }

        private Vector2 _lastMousePos;
        private IPanel _currentPanel;


        private bool _bindingHover;

        private void OnPointerEnter(PointerEnterEvent evt)
        {
            if (_bindingHover && _tooltipHover) return;
            if (!_currentTask.Status.IsCompleted()) return;

            _bindingHover = true;
            _lastMousePos = evt.position;
            ResetTime();
            _currentTask = ShowTooltipAsync(_cts.Token);
        }

        private VisualElement _currentParent;
        private UniTask _currentTask;
        private CancellationTokenSource _cts;


        private float timeTillShow;

        private void ResetTime()
        {
            timeTillShow = MVVMTKSettings.Instance.tooltipHoverTime / 1000f;
        }

        private async UniTask ShowTooltipAsync(CancellationToken ct)
        {
            while (timeTillShow > 0)
            {
                if (ct.IsCancellationRequested) return;
                timeTillShow -= Time.deltaTime;
                await UniTask.Yield();
            }

            ShowTooltip();
        }

        private void ShowTooltip()
        {
            var pos = _lastMousePos;
            var tooltipRect = _tooltip.layout;

            // In case layout was non calculated yet it will be NaN and we don't want NaN position
            if (tooltipRect.size.x > 0)
            {
                pos -= tooltipRect.size * 0.1f;
            }

            _tooltip.SetPosition(pos);

            _currentParent = _currentPanel.visualTree;
            _currentParent.Add(_tooltip);
            _tooltipHover = true;
        }

        private void HideTooltip()
        {
            if (_currentParent is not null)
            {
                _currentParent.Remove(_tooltip);
                _currentParent = null;
            }
            else if (!_currentTask.Status.IsCompleted())
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = new();
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (_currentTask.Status.IsCompleted()) return;
            ResetTime();
            _lastMousePos = evt.position;
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            _bindingHover = false;
            // If tooltip is still hovered, don't hide it
            if (_tooltipHover) return;

            HideTooltip();
        }

        private void OnTooltipEvent(TooltipChangedEvent evt) => _tooltip.Text = evt.tooltip;

        protected override void RegisterCallbacksOnTarget()
        {
            if (target.panel is not null)
            {
                _currentPanel = target.panel;
            }

            target.RegisterCallback<AttachToPanelEvent>(OnPanelAttached);
            target.RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);
            target.RegisterCallback<TooltipChangedEvent>(OnTooltipEvent);
            target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }

        private void OnPanelAttached(AttachToPanelEvent evt) => _currentPanel = evt.destinationPanel;

        private void OnPanelDetached(DetachFromPanelEvent evt) => HideTooltip();


        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<AttachToPanelEvent>(OnPanelAttached);
            target.UnregisterCallback<DetachFromPanelEvent>(OnPanelDetached);
            target.UnregisterCallback<TooltipChangedEvent>(OnTooltipEvent);
            target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        }
    }
}