using System;
using RandomEncounters.Utils;
using Unity.Entities;
using VRising.GameData;

namespace RandomEncounters.Components
{
    public class Timer : IDisposable
    {
        private bool _enabled;
        private bool _isRunning;
        private DateTime _lastRunTime;
        private TimeSpan _delay;
        private Action<World> _action;
        private Func<object,TimeSpan> _delayAction;

        public void Start(Action<World> action, TimeSpan delay)
        {
            _delay = delay;
            _lastRunTime = DateTime.UtcNow - delay;
            _action = action;
            _enabled = true;
            GameFrame.OnUpdate += GameFrame_OnUpdate;
        }


        public void Start(Action<World> action, Func<object, TimeSpan> delayAction)
        {
            _delayAction = delayAction;
            _delay = _delayAction.Invoke(1);
            _lastRunTime = DateTime.UtcNow;
            _action = action;
            _enabled = true;
            GameFrame.OnUpdate += GameFrame_OnUpdate;
        }

        private void GameFrame_OnUpdate()
        {
            Update(GameData.World);
        }
        private void Update(World world)
        {
            if (!_enabled || _isRunning)
            {
                return;
            }

            if (_lastRunTime + _delay >= DateTime.UtcNow)
            {
                return;
            }

            _isRunning = true;
            try
            {
                Plugin.Logger.LogDebug("Executing timer.");
                _action.Invoke(world);
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError(ex);
            }
            finally
            {
                var onlineUsersCount = DataFactory.GetOnlineUsersCount();
                if (_delayAction != null)
                {
                    _delay = _delayAction.Invoke(onlineUsersCount);
                }
                _lastRunTime = DateTime.UtcNow;
                _isRunning = false;
            }
        }

        public void Stop()
        {
            GameFrame.OnUpdate -= GameFrame_OnUpdate;
            _enabled = false;
        }

        public void Dispose()
        {
            if (_enabled)
            {
                Stop();
            }
        }
    }
}