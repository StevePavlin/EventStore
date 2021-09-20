﻿using EventStore.Core.Services;
using EventTypeId = System.UInt32;

namespace EventStore.Core.LogV3
{
	public class LogV3SystemEventTypes {

		public const EventTypeId FirstRealEventTypeNumber = 1024;
		public const EventTypeId EventTypeInterval = 1;

		public const EventTypeId EmptyEventTypeNumber = 0;
		public const EventTypeId StreamDeletedNumber = 1;
		public const EventTypeId StreamMetadataNumber = 2;
		public const EventTypeId StreamCreatedNumber = 3;
		public const EventTypeId EventTypeDefinedNumber = 4;
		
		public static bool TryGetSystemEventTypeId(string type, out EventTypeId eventTypeId) {
			switch (type) {
				case SystemEventTypes.EmptyEventType:
					eventTypeId = EmptyEventTypeNumber;
					return true;
				case SystemEventTypes.StreamDeleted:
					eventTypeId = StreamDeletedNumber;
					return true;
				case SystemEventTypes.StreamMetadata:
					eventTypeId = StreamMetadataNumber;
					return true;
				case SystemEventTypes.EventTypeDefined:
					eventTypeId = EventTypeDefinedNumber;
					return true;
				default:
					eventTypeId = EmptyEventTypeNumber;
					return false;
			}
		}
		
		public static bool TryGetVirtualEventType(EventTypeId eventTypeId, out string name) {
			if (!IsVirtualEventType(eventTypeId)) {
				name = null;
				return false;
			}

			name = eventTypeId switch {
				EmptyEventTypeNumber => SystemEventTypes.EmptyEventType,
				StreamMetadataNumber => SystemEventTypes.StreamMetadata,
				StreamCreatedNumber => SystemEventTypes.StreamCreated,
				StreamDeletedNumber => SystemEventTypes.StreamDeleted,
				EventTypeDefinedNumber => SystemEventTypes.EventTypeDefined,
				_ => null,
			};

			return name != null;
		}
		
		private static bool IsVirtualEventType(EventTypeId eventTypeId) => eventTypeId < FirstRealEventTypeNumber;
	}
}
