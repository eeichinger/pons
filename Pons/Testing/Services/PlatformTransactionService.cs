using System;
using Common.Logging;
using Spring.Transaction;
using Spring.Transaction.Support;
using Viz.Testing.Util;

namespace Viz.Testing.Services
{
    public class PlatformTransactionService : IServiceFactory, IDisposable
    {
        protected ILog logger = LogManager.GetLogger(typeof(PlatformTransactionService));

        /// <summary>
        /// The transaction manager to use
        /// </summary>
        protected IPlatformTransactionManager transactionManager;

        /// <summary>
        /// Should we roll back by default?
        /// </summary>
        private bool defaultRollback = true;

        /// <summary>
        /// Should we commit the current transaction?
        /// </summary>
        private bool complete = false;

        /// <summary>
        /// Number of transactions started
        /// </summary>
        private int transactionsStarted = 0;

        /// <summary>
        /// Default transaction definition is used.
        /// Subclasses can change this to cause different behaviour.
        /// </summary>
        private ITransactionDefinition transactionDefinition = new DefaultTransactionDefinition();

        /// <summary>
        /// TransactionStatus for this test. Typical subclasses won't need to use it.
        /// </summary>
        protected ITransactionStatus transactionStatus;


        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformTransactionService"/> class.
        /// </summary>
        public PlatformTransactionService(IPlatformTransactionManager transactionManager)
        {
            Ensure.NotNull(transactionManager, "transactionManager");
            this.transactionManager = transactionManager;
        }

        /// <summary>
        /// Sets the transaction manager to use.
        /// </summary>
        public IPlatformTransactionManager TransactionManager
        {
            set { transactionManager = value; }
        }

        /// <summary>
        /// Sets the default rollback flag.
        /// </summary>
        public bool DefaultRollback
        {
            set { defaultRollback = value; }
        }

        /// <summary>
        /// Set the <see cref="ITransactionDefinition"/> to be used
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DefaultTransactionDefinition"/>
        /// </remarks>
        public ITransactionDefinition TransactionDefinition
        {
            set { transactionDefinition = value; }
            get { return transactionDefinition; }
        }

        /// <summary>
        /// Prevents the transaction.
        /// </summary>
        public virtual void PreventTransaction()
        {
            this.transactionDefinition = null;
        }


        /// <summary>
        /// Creates a transaction
        /// </summary>
        public IDisposable GetService(object fixtureInstance)
        {
            OnSetUp();
            return this;
        }

        public void Dispose()
        {
            OnTearDown();
        }

        protected void OnSetUp()
        {
            this.complete = !this.defaultRollback;

            if (this.transactionManager == null)
            {
                logger.Info("No transaction manager set: test will NOT run within a transaction");
            }
            else if (this.transactionDefinition == null)
            {
                logger.Info("No transaction definition set: test will NOT run within a transaction");
            }
            else
            {
                OnSetUpBeforeTransaction();
                StartNewTransaction();
                try
                {
                    OnSetUpInTransaction();
                }
                catch (Exception)
                {
                    EndTransaction();
                    throw;
                }
            }
        }


        /// <summary>
        /// Callback method called before transaction is setup.
        /// </summary>
        protected virtual void OnSetUpBeforeTransaction()
        {
        }

        /// <summary>
        /// Callback method called after transaction is setup.
        /// </summary>
        protected virtual void OnSetUpInTransaction()
        {
        }

        /// <summary>
        /// rollback the transaction.
        /// </summary>
        protected void OnTearDown()
        {
            // Call onTearDownInTransaction and end transaction if the transaction is still active.
            if (this.transactionStatus != null && !this.transactionStatus.Completed)
            {
                try
                {
                    OnTearDownInTransaction();
                }
                finally
                {
                    EndTransaction();
                }
            }
            // Call onTearDownAfterTransaction if there was at least one transaction,
            // even if it has been completed early through an endTransaction() call.
            if (this.transactionsStarted > 0)
            {
                OnTearDownAfterTransaction();
            }
        }

        /// <summary>
        /// Callback before rolling back the transaction.
        /// </summary>
        protected virtual void OnTearDownInTransaction()
        {
        }

        /// <summary>
        /// Callback after rolling back the transaction.
        /// </summary>
        protected virtual void OnTearDownAfterTransaction()
        {
        }

        /// <summary>
        /// Set the complete flag..
        /// </summary>
        public virtual void SetComplete()
        {
            if (this.transactionManager == null)
            {
                throw new InvalidOperationException("No transaction manager set");
            }
            this.complete = true;
        }

        /// <summary>
        /// Ends the transaction according to configuration. By default peforms a rollback.
        /// </summary>
        public virtual void EndTransaction()
        {
            EndTransaction(this.complete);
        }

        /// <summary>
        /// Ends the transaction.
        /// </summary>
        /// <param name="complete">Pass in <c>true</c> to commit the ambient transaction, if any. 
        /// <c>false</c> will rollback the transaction.
        /// </param>
        public virtual void EndTransaction(bool complete)
        {
            if (this.transactionStatus != null)
            {
                try
                {
                    if (!complete)
                    {
                        this.transactionManager.Rollback(this.transactionStatus);
                        logger.Info("Rolled back transaction after test execution");
                    }
                    else
                    {
                        this.transactionManager.Commit(this.transactionStatus);
                        logger.Info("Committed transaction after test execution");
                    }
                }
                finally
                {
                    this.transactionStatus = null;
                }
            }
        }

        /// <summary>
        /// Starts the new transaction.
        /// </summary>
        protected void StartNewTransaction()
        {
            if (this.transactionStatus != null)
            {
                throw new InvalidOperationException("Cannot start new transaction without ending existing transaction: " +
                                                    "Invoke endTransaction() before startNewTransaction()");
            }
            if (this.transactionManager == null)
            {
                throw new InvalidOperationException("No transaction manager set");
            }

            this.transactionStatus = this.transactionManager.GetTransaction(this.transactionDefinition);
            ++this.transactionsStarted;
            this.complete = !this.defaultRollback;

            if (logger.IsInfoEnabled)
            {
                logger.Info("Began transaction (" + this.transactionsStarted + "): transaction manager [" +
                            this.transactionManager + "]; default rollback = " + this.defaultRollback);
            }
        }
    }
}