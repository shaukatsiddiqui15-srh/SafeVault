using System.Data;
using System.Data.Common;
using NUnit.Framework;

#nullable disable

namespace SafeVault.Tests;

[TestFixture]
public class TestInputValidation
{
    [Test]
    public void TestForSQLInjection()
    {
        var connection = new FakeDbConnection();
        var repository = new UserRepository();
        var command = repository.CreateFindUserByUsernameCommand(connection, "alice' OR '1'='1");

        Assert.That(command.CommandText, Does.Contain("WHERE Username = @Username"));
        Assert.That(command.CommandText, Does.Not.Contain("OR '1'='1"));
        Assert.That(command.Parameters, Has.Count.EqualTo(1));
        Assert.That(command.Parameters[0].ParameterName, Is.EqualTo("@Username"));
        Assert.That(command.Parameters[0].Value, Is.EqualTo("aliceOR11"));
    }

    [Test]
    public void TestForXSS()
    {
        var sanitized = InputSanitizer.SanitizeUsername("<script>alert('xss')</script>");
        var encoded = InputSanitizer.HtmlEncodeForDisplay("<script>alert('xss')</script>");

        Assert.That(sanitized, Is.EqualTo("scriptalertxssscript"));
        Assert.That(encoded, Is.EqualTo("&lt;script&gt;alert(&#39;xss&#39;)&lt;/script&gt;"));
        Assert.That(encoded, Does.Not.Contain("<script>"));
    }

    [Test]
    public void TestEmailValidationRejectsWhitespaceAndScriptPayloads()
    {
        Assert.That(() => InputSanitizer.SanitizeEmail("alice@example.com<script>"),
            Throws.ArgumentException);
        Assert.That(InputSanitizer.SanitizeEmail(" alice@example.com "), Is.EqualTo("alice@example.com"));
    }

    private sealed class FakeDbConnection : DbConnection
    {
        private string _connectionString = string.Empty;
        private ConnectionState _state = ConnectionState.Closed;

        public override string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value ?? string.Empty;
        }

        public override string Database => "SafeVault";
        public override string DataSource => "Fake";
        public override string ServerVersion => "1.0";
        public override ConnectionState State => _state;

        public override void ChangeDatabase(string databaseName) { }

        public override void Close()
        {
            _state = ConnectionState.Closed;
        }

        public override void Open()
        {
            _state = ConnectionState.Open;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();

        protected override DbCommand CreateDbCommand() => new FakeDbCommand(this);
    }

    private sealed class FakeDbCommand : DbCommand
    {
        private readonly FakeDbConnection _connection;
        private readonly FakeDbParameterCollection _parameters = new();
        private string _commandText = string.Empty;

        public FakeDbCommand(FakeDbConnection connection)
        {
            _connection = connection;
        }

        public override string CommandText
        {
            get => _commandText;
            set => _commandText = value ?? string.Empty;
        }

        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; } = CommandType.Text;
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection DbConnection
        {
            get => _connection;
            set => throw new NotSupportedException();
        }

        protected override DbParameterCollection DbParameterCollection => _parameters;

        protected override DbTransaction DbTransaction { get; set; }

        public override void Cancel() { }

        public override int ExecuteNonQuery() => throw new NotSupportedException();

        public override object ExecuteScalar() => throw new NotSupportedException();

        public override void Prepare() { }

        protected override DbParameter CreateDbParameter() => new FakeDbParameter();

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotSupportedException();
    }

    private sealed class FakeDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _parameters = new();

        public override int Count => _parameters.Count;
        public override object SyncRoot => ((System.Collections.ICollection)_parameters).SyncRoot!;

        public override int Add(object value)
        {
            _parameters.Add((DbParameter)value);
            return _parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var value in values)
            {
                Add(value!);
            }
        }

        public override void Clear() => _parameters.Clear();

        public override bool Contains(object value) => _parameters.Contains((DbParameter)value);

        public override bool Contains(string value) => _parameters.Any(parameter => parameter.ParameterName == value);

        public override void CopyTo(Array array, int index) => _parameters.ToArray().CopyTo(array, index);

        public override System.Collections.IEnumerator GetEnumerator() => _parameters.GetEnumerator();

        public override int IndexOf(object value) => _parameters.IndexOf((DbParameter)value);

        public override int IndexOf(string parameterName) => _parameters.FindIndex(parameter => parameter.ParameterName == parameterName);

        public override void Insert(int index, object value) => _parameters.Insert(index, (DbParameter)value);

        public override void Remove(object value) => _parameters.Remove((DbParameter)value);

        public override void RemoveAt(int index) => _parameters.RemoveAt(index);

        public override void RemoveAt(string parameterName) => _parameters.RemoveAll(parameter => parameter.ParameterName == parameterName);

        protected override DbParameter GetParameter(int index) => _parameters[index];

        protected override DbParameter GetParameter(string parameterName) => _parameters.First(parameter => parameter.ParameterName == parameterName);

        protected override void SetParameter(int index, DbParameter value) => _parameters[index] = value;

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index >= 0)
            {
                _parameters[index] = value;
            }
            else
            {
                _parameters.Add(value);
            }
        }
    }

    private sealed class FakeDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; } = string.Empty;
        public override string SourceColumn { get; set; } = string.Empty;
        public override object Value { get; set; } = string.Empty;
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }

        public override void ResetDbType() { }
    }
}

#nullable restore
