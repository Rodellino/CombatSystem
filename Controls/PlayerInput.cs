using System;

// TODO: Properly handle exception in GetValue, line 45

// IMPORTANT NOTE: dynamic keyword is used in line 29 to store input value, but causes .NET
// compatibility issues. This has to be acknowledged in Unity under "Project Settings > Player"
// by setting Api Compatibility Level to .NET 4.x and Scripting Backend IL2CPP

/// <summary>
/// Player input data
/// </summary>
public class PlayerInput
{
    public enum Action { Move, Dash, MeleeAttack }

    public enum Status { Started, Performed, Canceled }

    public struct Input
    {
        public Action Action { get { return _action; } }
        private readonly Action _action;

        public Status Status { get { return _status; } }
        private readonly Status _status;
        public bool Started { get { return _status.Equals(Status.Started); } }
        public bool Performed { get { return _status.Equals(Status.Performed); } }
        public bool Canceled { get { return _status.Equals(Status.Canceled); } }

        private readonly dynamic _value;

        public Input(Action action, Status status)
        {
            this._action = action;
            this._status = status;
            this._value = default;
        }

        public Input(Action action, Status status, dynamic value)
        {
            this._action = action;
            this._status = status;
            this._value = value;
        }

        public TValue GetValue<TValue>()
        {
            //TODO: Convert throws exception on unmatching type, handle properly
            return (TValue)Convert.ChangeType(_value, typeof(TValue));
        }
    }
}