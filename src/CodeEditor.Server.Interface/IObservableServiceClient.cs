using System;
using System.Collections.Generic;
using System.Net;
using CodeEditor.Reactive;
using CodeEditor.Reactive.Disposables;
using ServiceStack.ServiceClient.Web;
using ServiceStack.ServiceHost;

namespace CodeEditor.Server.Interface
{
	public interface IObservableServiceClient
	{
		IObservableX<TResponse> ObserveMany<TResponse>(IReturn<IEnumerable<TResponse>> request);
	}

	public class ObservableServiceClient : IObservableServiceClient
	{
		private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(3);

		readonly string _baseUri;

		public ObservableServiceClient(string baseUri)
		{
			_baseUri = baseUri;
		}

		public IObservableX<TResponse> ObserveMany<TResponse>(IReturn<IEnumerable<TResponse>> request)
		{
			return ObservableX.Create<TResponse>(observer =>
			{
				var client = new JsonServiceClient(_baseUri) {Timeout = Timeout};
				var disposable = new MultipleAssignmentDisposable
				{
					Disposable = Disposable.Create(client.CancelAsync)
				};

				client.SendAsync<HttpWebResponse>(
					request,
					onSuccess: response =>
					{
						var responseStream = response.GetResponseStream();
						disposable.Disposable = responseStream.DeserializeMany<TResponse>().Subscribe(observer);
					},
					onError: (response, exception) =>
					{
						disposable.Disposable = null;
						observer.OnError(exception);
					});

				return disposable.Dispose;
			});
		}
	}
}