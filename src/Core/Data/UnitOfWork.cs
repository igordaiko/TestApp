using System;
using Luxena;

namespace ImportCoordinator.Core.Data
{
    public class UnitOfWork : IDisposable
    {
        public UnitOfWork(bool transactional, Action onClose)
        {
            _onClose = onClose;

            _database = _pool.Acquire();

            if (!transactional)
                return;

            _database.BeginTransaction();

            _transactional = true;

            _closeTransaction = true;
        }

        public UnitOfWork(UnitOfWork parent, bool transactional)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            _database = parent.Database;

            if (!transactional)
                return;

            _transactional = true;

            if (_database.Transaction != null)
                return;

            _database.BeginTransaction();

            _closeTransaction = true;
        }

        public Database Database
        {
            get
            {
                CheckDisposed();

                return _database;
            }
        }

        public void Dispose()
        {
            if (_database == null)
                return;

            Close();
        }

        public void Commit()
        {
            CheckClose();

            if (_closeTransaction)
                _database.CommitTransaction();

            Close();
        }

        public void Rollback()
        {
            CheckClose();

            if (_closeTransaction)
                _database.RollbackTransaction();

            Close();
        }

        private void CheckDisposed()
        {
            if (_database == null)
                throw new ObjectDisposedException(null);
        }

        private void CheckClose()
        {
            CheckDisposed();

            if (!_transactional)
                throw new InvalidOperationException("Non transactional");
        }

        private void Close()
        {
            if (_onClose != null)
            {
                _pool.Release(_database);

                _onClose();
            }

            _database = null;
        }

        private static readonly Pool<Database> _pool = new Pool<Database>(() => Database.Open());

        private readonly Action _onClose;

        private readonly bool _transactional;

        private readonly bool _closeTransaction;

        private Database _database;
    }
}
