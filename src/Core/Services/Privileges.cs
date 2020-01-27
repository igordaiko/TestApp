using System;
using System.Collections.Generic;
using ImportCoordinator.Core.Data;

namespace ImportCoordinator.Core.Services
{
    internal class Privileges
    {
        public static readonly Privileges Empty = new Privileges();

        private Privileges()
        {
        }

        public bool PutEmailOnQueue { get; private set; }

        public bool PutSmsOnQueue { get; private set; }

        public bool GetEmail { get; private set; }

        public bool GetSms { get; private set; }

        public static Privileges Create(long roles)
        {
            var result = new Privileges();

            foreach (var pair in _rolesMapping)
                if ((roles & (int)pair.Key) != 0)
                    pair.Value(result);

            return result;
        }

        private static readonly Dictionary<Roles, Action<Privileges>> _rolesMapping = new Dictionary<Roles, Action<Privileges>>
        {
            {
                Roles.MainApp, p =>
                {
                    p.PutEmailOnQueue = true;
                    p.PutSmsOnQueue = true;
                }
            },
            {
                Roles.Gateway, p =>
                {
                    p.GetEmail = true;
                    p.GetSms = true;
                }
            }
        };
    }
}
