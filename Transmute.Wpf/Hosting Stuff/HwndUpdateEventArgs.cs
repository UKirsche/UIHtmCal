#region File Description//-----------------------------------------------------------------------------
// HwndMouseEventArgs.cs
//
// Copyright 2011, Aaron Foley.
// Licensed under the terms of the Ms-PL: http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;

namespace Transmute.Wpf
{
    /// <summary>
    /// Event Args used for Update Events in the GraphicsDeviceControl
    /// </summary>
    public class HwndUpdateEventArgs : EventArgs
    {
        /// <summary>
        ///  Snapshot of the game timing state expressed in values that can be used by variable-step (real time) or fixed-step (game time) games.
        /// </summary>
        public GameTime GameTime { get; private set; }

        public HwndUpdateEventArgs(GameTime gameTime)
        {
            GameTime = gameTime;
        }
    }
}
