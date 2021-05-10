﻿using System;
using EventStore.Core.Data;
using EventStore.Core.Services.Storage.ReaderIndex;
using NUnit.Framework;

namespace EventStore.Core.Tests.Services.Storage.Idempotency {
	[TestFixture]
	public class when_writing_a_second_event_after_the_first_event_has_been_replicated<TLogFormat,TStreamId> : WriteEventsToIndexScenario<TLogFormat,TStreamId>{
		private Guid _eventId = Guid.NewGuid();
		private TStreamId _streamId;
        public override void WriteEvents()
        {
			var expectedEventNumber = -1;
			var transactionPosition = 1000;
			var prepares = CreatePrepareLogRecord("stream", expectedEventNumber, "type", _eventId, transactionPosition);
			var commit = CreateCommitLogRecord(transactionPosition + RecordOffset, transactionPosition, expectedEventNumber + 1);

			_logFormat.StreamNameIndex.GetOrAddId("stream", out _streamId, out _, out _);

			/*First write: committed to db and index*/
			WriteToDB(prepares);
			PreCommitToIndex(prepares);
			
			WriteToDB(commit);
			PreCommitToIndex(commit);
			
			CommitToIndex(prepares);
			CommitToIndex(commit);
        }

		[Test]
		public void check_commit_with_same_expectedversion_should_return_idempotent_decision() {
			/*Second, idempotent write*/
			var commitCheckResult = _indexWriter.CheckCommit(_streamId, -1, new Guid[] { _eventId });
			Assert.AreEqual(CommitDecision.Idempotent, commitCheckResult.Decision);
		}

		[Test]
		public void check_commit_with_expectedversion_any_should_return_idempotent_decision() {
			/*Second, idempotent write*/
			var commitCheckResult = _indexWriter.CheckCommit(_streamId, ExpectedVersion.Any, new Guid[] { _eventId });
			Assert.AreEqual(CommitDecision.Idempotent, commitCheckResult.Decision);
		}

		[Test]
		public void check_commit_with_next_expectedversion_should_return_ok_decision() {
			var commitCheckResult = _indexWriter.CheckCommit(_streamId, 0, new Guid[] { _eventId });
			Assert.AreEqual(CommitDecision.Ok, commitCheckResult.Decision);
		}

		[Test]
		public void check_commit_with_incorrect_expectedversion_should_return_wrongexpectedversion_decision() {
			var commitCheckResult = _indexWriter.CheckCommit(_streamId, 1, new Guid[] { _eventId });
			Assert.AreEqual(CommitDecision.WrongExpectedVersion, commitCheckResult.Decision);
		}
    }
}
