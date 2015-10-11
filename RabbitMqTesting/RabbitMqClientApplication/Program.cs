using System;
using RabbitMqTesting;
using RabbitMQ.Client;
using FizzWare.NBuilder;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace RabbitMqClientApplication
{
	class CustomConnectionFactory : ConnectionFactory, IDisposable
	{
		private List<string> _hosts;

		//private RandomItemPicker<string> _random;
		private Random _random;
		
		private IConnection _connection;
		
		public CustomConnectionFactory()
		{
			_hosts = new List<string>() { "10.0.0.10", "10.0.0.11" };
			//_random = new FizzWare.NBuilder.RandomItemPicker<string>(_hosts, new RandomGenerator());
			_random = new Random();
			HostName = _hosts[0];
		}

		private IConnection GetNewConnection()
		{
			var aliveHosts = _hosts.OrderBy(_ => _random.Next()).ToList();
			var exceptions = new List<Exception>();
			foreach (var host in aliveHosts)
			{
				this.RequestedConnectionTimeout = 10;
				HostName = host;
				try
				{
					System.Diagnostics.Debug.WriteLine("Trying connection to host {0}", new object[] {
						HostName
					});
					return base.CreateConnection();
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("Failed connection to host {0}", new object[] {
						HostName
					});
					exceptions.Add(e);
					Thread.Sleep(this.RequestedConnectionTimeout);
				}
			}
			throw new AggregateException("Could not get connection from list of hosts", exceptions);
		}

		public override IConnection CreateConnection()
		{ 
			if (_connection == null || !_connection.IsOpen)
			{
				_connection = GetNewConnection();
			}
			return _connection;
		}
		#region IDisposable implementation
		public void Dispose()
		{
			if (_connection != null)
			{
				_connection.Dispose();
			}
		}
		#endregion
    }
		
	class MainClass
	{
		public static void Main (string[] args)
		{
			using(var factory = new CustomConnectionFactory () { 
				HostName = "10.0.0.10", 
				UserName = "dotnet", 
				Password = "dotnet", 
				VirtualHost = "/", 
				AutomaticRecoveryEnabled = false,
				UseBackgroundThreadsForIO = false,
				RequestedHeartbeat = 0})
			using(var sender = new RabbitMqNotificationPublisher("customer1", factory, "flights"))
			foreach(var message in StringIterator())
			{
				Console.WriteLine("Sending message {0}", message);
				try
				{
					sender.SendMessages(message);
				}
				catch(Exception e)
				{
					Console.WriteLine("Failed to send message {0}\n {1}", message, e.Message);
				}
			}
			/*
			
			var messageBacklog = new List<string>();
			var backlogLock = new object();
			var messageThread = new Thread(() => {
				foreach (var message in StringIterator())
				{
					var factory = new ConnectionFactory () { 
						HostName = "10.0.0.10", 
						UserName = "dotnet", 
						Password = "dotnet", 
						VirtualHost = "/", 
						AutomaticRecoveryEnabled = false,
						UseBackgroundThreadsForIO = false,
						RequestedHeartbeat = 0};
					var sender = new RabbitMqNotificationPublisher ("customer1", factory, "flights");		
					try
					{
						Console.WriteLine("Sending message {0}", message);
						sender.SendMessages(message);
					}
					catch
					{
						lock(backlogLock)
						{
							messageBacklog.Add(message);
						}
					}
				}
			});
			messageThread.Start();
			while (true)
			{
				lock (backlogLock)
				{
					if (messageBacklog.Count == 0)
					{
						Thread.Sleep(4000);
					}
					Console.WriteLine("Sending backlog of {0} messages", messageBacklog.Count);
				}
				IList<string> messages;
				lock(backlogLock)
				{
					messages = new List<string>(messageBacklog);
				}
				foreach(var backlogMessage in messages)
				{
					var factory = new ConnectionFactory () { 
						HostName = "10.0.0.10", 
						UserName = "dotnet", 
						Password = "dotnet", 
						VirtualHost = "/", 
						AutomaticRecoveryEnabled = false,
						UseBackgroundThreadsForIO = false,
						RequestedHeartbeat = 0};
					var sender = new RabbitMqNotificationPublisher ("customer1", factory, "flights");
					var err = false;
					try
					{
						Console.WriteLine("Sending backlogged message {0}", backlogMessage);
						sender.SendMessages(backlogMessage);
					}
					catch(Exception e)
					{
						Console.WriteLine("Failed to send message", e);
						err = true;
					}
					finally
					{
						if (!err)
						{
							lock(backlogLock)
							{
								messageBacklog.Remove(backlogMessage);	
							}
						}
					}
				}
			}*/
		}

		public static IEnumerable<string> StringIterator() {
			while (true) {
				Thread.Sleep(3);
				yield return Guid.NewGuid().ToString ();
			}
		}
	}
}
