// Copyright (c) 2015, Alexander Endris
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// * Neither the name of EditorDrag nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System.IO;
using System.Windows;
using System.Windows.Input;

using Microsoft.VisualStudio.Text.Editor;
using MouseDrag;

namespace EditorDrag
{
    public class MouseProcessor : MouseProcessorBase
    {
        private readonly IWpfTextView _wpfTextView;

        private readonly Cursor _originalCursor;

        private bool _down;

        private Point _initialPosition;

        public MouseProcessor(IWpfTextView wpfTextView)
        {
            _wpfTextView = wpfTextView;
            _originalCursor = wpfTextView.VisualElement.Cursor;
        }

        public override void PreprocessMouseUp(MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                _down = false;

                _wpfTextView.VisualElement.Cursor = _originalCursor;
            }

            base.PreprocessMouseUp(e);
        }

        public override void PreprocessMouseMove(MouseEventArgs e)
        {
            if (_down)
            {
                var newPosition = e.GetPosition(_wpfTextView.VisualElement);
                var offset = _initialPosition  - newPosition;
                
                _wpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(offset.X);
                _wpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(-offset.Y);

                _initialPosition = newPosition;
            }

            base.PreprocessMouseMove(e);
        }

        public override void PreprocessMouseDown(MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _down = true;
                _initialPosition = e.GetPosition(_wpfTextView.VisualElement);

                _wpfTextView.VisualElement.Cursor = GetGrabCursor();
            }

            base.PreprocessMouseDown(e);
        }

        private Cursor GetGrabCursor()
        {
            var memoryStream = new MemoryStream(Resources.GrabCursor);

            return new Cursor(memoryStream);
        }
    }
}