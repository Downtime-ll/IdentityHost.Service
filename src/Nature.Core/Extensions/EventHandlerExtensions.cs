using System;

namespace Nature.Core.Extensions
{
    public static class EventHandlerExtensions
    {
        /// <summary>
        /// 安全执行
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <param name="sender"></param>
        public static void InvokeSafely(this EventHandler eventHandler, object sender)
        {
            eventHandler.InvokeSafely(sender, EventArgs.Empty);
        }

        /// <summary>
        /// 安全执行
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void InvokeSafely(this EventHandler eventHandler, object sender, EventArgs e)
        {
            eventHandler?.Invoke(sender, e);
        }

        /// <summary>
        /// 安全执行
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventHandler"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void InvokeSafely<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e)
           where TEventArgs : EventArgs
        {
            eventHandler?.Invoke(sender, e);
        }
    }
}