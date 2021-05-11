using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using ZoomNet.Models.Webhooks;
using static ZoomNet.Internal;

namespace ZoomNet.Utilities
{
	/// <summary>
	/// Converts a JSON string received from a webhook into and array of <see cref="Event">events</see>.
	/// </summary>
	/// <seealso cref="Newtonsoft.Json.JsonConverter" />
	internal class WebHookEventConverter : JsonConverter
	{
		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Event);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanRead
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanWrite
		{
			get { return false; }
		}

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>
		/// The object value.
		/// </returns>
		/// <exception cref="System.Exception">Unable to determine the field type.</exception>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var jsonObject = JObject.Load(reader);

			jsonObject.TryGetValue("event", StringComparison.OrdinalIgnoreCase, out JToken eventTypeJsonProperty);
			jsonObject.TryGetValue("payload", StringComparison.OrdinalIgnoreCase, out JToken payloadJsonProperty);
			jsonObject.TryGetValue("event_ts", StringComparison.OrdinalIgnoreCase, out JToken timestamptJsonProperty);

			var eventType = (EventType)eventTypeJsonProperty.ToObject(typeof(EventType));

			Event webHookEvent;
			switch (eventType)
			{
				case EventType.MeetingCreated:
					webHookEvent = payloadJsonProperty.ToObject<MeetingCreatedEvent>(serializer);
					break;
				//	case EventType.Click:
				//		webHookEvent = jsonObject.ToObject<ClickedEvent>(serializer);
				//		break;
				//	case EventType.Deferred:
				//		webHookEvent = jsonObject.ToObject<DeferredEvent>(serializer);
				//		break;
				//	case EventType.Delivered:
				//		webHookEvent = jsonObject.ToObject<DeliveredEvent>(serializer);
				//		break;
				//	case EventType.Dropped:
				//		webHookEvent = jsonObject.ToObject<DroppedEvent>(serializer);
				//		break;
				//	case EventType.GroupResubscribe:
				//		webHookEvent = jsonObject.ToObject<GroupResubscribeEvent>(serializer);
				//		break;
				//	case EventType.GroupUnsubscribe:
				//		webHookEvent = jsonObject.ToObject<GroupUnsubscribeEvent>(serializer);
				//		break;
				//	case EventType.Open:
				//		webHookEvent = jsonObject.ToObject<OpenedEvent>(serializer);
				//		break;
				//	case EventType.Processed:
				//		webHookEvent = jsonObject.ToObject<ProcessedEvent>(serializer);
				//		break;
				//	case EventType.SpamReport:
				//		webHookEvent = jsonObject.ToObject<SpamReportEvent>(serializer);
				//		break;
				//	case EventType.Unsubscribe:
				//		webHookEvent = jsonObject.ToObject<UnsubscribeEvent>(serializer);
				//		break;
				default:
					throw new Exception($"{eventTypeJsonProperty} is an unknown event type");
			}

			webHookEvent.EventType = eventType;
			webHookEvent.TimeStamp = timestamptJsonProperty.ToObject<long>().FromUnixTime(UnixTimePrecision.Milliseconds);

			return webHookEvent;
		}
	}
}
