using System;

namespace NEvilES.Pipeline
{
    public class CommandContext
    {
        public static CommandContext Null()
        {
            return new CommandContext(User.NullUser(), null, User.NullUser(), null);
        }

        public CommandContext(IUser by, ITransaction transaction, IUser impersonatorBy, string appVersion)
        {
            By = by;
            Transaction = transaction;
            ImpersonatorBy = impersonatorBy;
            AppVersion = appVersion;
            Result = new CommandResult();
        }

        public IUser By { get; set; }
        public ITransaction Transaction { get; set; }
        public IUser ImpersonatorBy { get; set; }
        public string AppVersion { get; set; }
        public CommandResult Result { get; set; }

        public class User : IUser
        {
            public static User NullUser()
            {
                return new User(Guid.Empty, null);
            }

            public User(Guid id)
            {
                GuidId = id;
            }

            public User(Guid id, int? user)
            {
                GuidId = id;
                UserId = user != 0 ? user : null;
            }

            public Guid GuidId { get; private set; }
            public int? UserId { get; set; }
        }

        public interface IUser
        {
            Guid GuidId { get; }
        }

        public abstract class TransactionBase : ITransaction
        {
            public Guid Id { get; protected set; }
        }

        public interface ITransaction
        {
            Guid Id { get; }
        }
    }
}