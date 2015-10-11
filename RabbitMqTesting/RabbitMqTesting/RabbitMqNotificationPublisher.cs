using System;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace RabbitMqTesting
{
	public class RabbitMqClientBase : IDisposable
	{
		protected string _queueName { get; private set; }
		
		protected IConnectionFactory _factory { get; private set; }
		
		protected RabbitMqClientBase(IConnectionFactory factory, string queueName)
		{
			_factory = factory;
			_queueName = queueName;
		}
		
		protected void ExecuteAgainstConnection(Action<IModel> action)
		{
			var connection = _factory.CreateConnection();
			using (var channel = connection.CreateModel())
			{
				System.Diagnostics.Debug.WriteLine("Opened channel");
				System.Diagnostics.Debug.WriteLine("Exec action");
				action(channel);
				System.Diagnostics.Debug.WriteLine("Action complete");
			}
		}
			
		
		
		#region IDisposable implementation
		public void Dispose()
		{
		}
		#endregion
	}
	
	public class RabbitMqNotificationPublisher : RabbitMqClientBase
	{
		private string _customer { get; set; }

		public RabbitMqNotificationPublisher(string customer, IConnectionFactory factory, string queueName)
			: base(factory, queueName)
		{
			_customer = customer;
		}


		public void SendMessages(params string[] messages)
		{
			ExecuteAgainstConnection(channel => 
            {
				channel.QueueDeclare(queue: _queueName, 
				                     durable: true, 
				                     exclusive: false, 
				                     autoDelete: false, 
				                     arguments: null);
				
				channel.ConfirmSelect();
				
				foreach(var message in messages)
				{
					var body = Encoding.UTF8.GetBytes(message);

					channel.BasicPublish(exchange: "", 
					                     routingKey: _queueName, 
					                     basicProperties: null, 
					                     body: body);
				}

				channel.WaitForConfirms();
			});
		}
	}
}
