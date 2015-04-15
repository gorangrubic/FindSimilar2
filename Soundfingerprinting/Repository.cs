﻿using System;
using System.Collections.Generic;
using System.Linq;

using Soundfingerprinting.Fingerprinting;
using Soundfingerprinting.Fingerprinting.WorkUnitBuilder;
using Soundfingerprinting.Hashing;

using Soundfingerprinting.Dao.Entities;
using Soundfingerprinting.DbStorage;
using Soundfingerprinting.DbStorage.Entities;

using Soundfingerprinting.SoundTools;

using Mirage;

using FindSimilar2; // SplashScreen and QueryResults

namespace Soundfingerprinting
{
	/// <summary>
	/// Singleton class for repository container
	/// </summary>
	public class Repository
	{
		/// <summary>
		///   Min hasher
		/// </summary>
		private readonly MinHash minHash;

		/// <summary>
		///   Storage for min-hash permutations
		/// </summary>
		private readonly IPermutations permutations;
		
		private DatabaseService dbService;
		private FingerprintService fingerprintService;
		
		private static int MAX_SIGNATURE_COUNT = 5; // the number of signatures to reduce to
		
		public Repository(IPermutations permutations, DatabaseService dbService, FingerprintService fingerprintService)
		{
			this.permutations = permutations;
			this.minHash = new MinHash(this.permutations);
			this.dbService = dbService;
			this.fingerprintService = fingerprintService;
		}
		
		public FingerprintService FingerprintService { get { return this.fingerprintService; } }

		public DatabaseService DatabaseService { get { return this.dbService; } }
		
		public MinHash MinHash { get { return this.minHash; } }
		
		/// <summary>
		/// Find Similar Tracks using passed audio file as input
		/// </summary>
		/// <param name="lshHashTables">Number of hash tables from the database</param>
		/// <param name="lshGroupsPerKey">Number of groups per hash table</param>
		/// <param name="thresholdTables">Threshold percentage [0.07 for 20 LHash Tables, 0.17 for 25 LHashTables]</param>
		/// <param name="param">Audio File Work Unit Parameter Object</param>
		/// <returns>a dictionary of perceptually similar tracks</returns>
		public Dictionary<Track, QueryStats> FindSimilarFromAudioFile(
			int lshHashTables,
			int lshGroupsPerKey,
			int thresholdTables,
			WorkUnitParameterObject param) {
			
			// Get fingerprints
			// TODO: Note that this method might return too few samples
			double[][] LogSpectrogram;
			List<bool[]> signatures = fingerprintService.CreateFingerprintsFromAudioFile(param, out LogSpectrogram);

			long elapsedMiliseconds = 0;
			
			// Query the database using Min Hash
			Dictionary<int, QueryStats> allCandidates = QueryFingerprintManager.QueryOneSongMinHash(
				signatures,
				dbService,
				minHash,
				lshHashTables,
				lshGroupsPerKey,
				thresholdTables,
				ref elapsedMiliseconds);

			IEnumerable<int> ids = allCandidates.Select(p => p.Key);
			IList<Track> tracks = dbService.ReadTrackById(ids);

			// Order by Hamming Similarity
			// Using PLINQ
			//OrderedParallelQuery<KeyValuePair<int, QueryStats>> order = allCandidates.AsParallel()
			IOrderedEnumerable<KeyValuePair<int, QueryStats>> order = allCandidates
				.OrderBy((pair) => pair.Value.OrderingValue =
				         pair.Value.HammingDistance / pair.Value.NumberOfTotalTableVotes
				         + 0.4 * pair.Value.MinHammingDistance);
			
			// Join on the ID properties.
			var joined = from o in order
				join track in tracks on o.Key equals track.Id
				select new { track, o.Value };

			Dictionary<Track, QueryStats> stats = joined.ToDictionary(Key => Key.track, Value => Value.Value);
			
			return stats;
		}
		
		/// <summary>
		/// Find Similar Tracks using passed audio samples as input and return a Dictionary
		/// </summary>
		/// <param name="lshHashTables">Number of hash tables from the database</param>
		/// <param name="lshGroupsPerKey">Number of groups per hash table</param>
		/// <param name="thresholdTables">Minimum number of hash tables that must be found for one signature to be considered a candidate (0 and 1 = return all candidates, 2+ = return only exact matches)</param>
		/// <param name="param">Audio File Work Unit Parameter Object</param>
		/// <returns>a dictionary of perceptually similar tracks</returns>
		public Dictionary<Track, double> FindSimilarFromAudioSamples(
			int lshHashTables,
			int lshGroupsPerKey,
			int thresholdTables,
			WorkUnitParameterObject param) {
			
			var t = new DbgTimer();
			t.Start ();
			
			// Get fingerprints
			double[][] logSpectrogram;
			List<bool[]> signatures = fingerprintService.CreateFingerprintsFromAudioSamples(param.AudioSamples, param, out logSpectrogram);
			
			long elapsedMiliseconds = 0;
			
			// Query the database using Min Hash
			Dictionary<int, QueryStats> allCandidates = QueryFingerprintManager.QueryOneSongMinHash(
				signatures,
				dbService,
				minHash,
				lshHashTables,
				lshGroupsPerKey,
				thresholdTables,
				ref elapsedMiliseconds);

			IEnumerable<int> ids = allCandidates.Select(p => p.Key);
			IList<Track> tracks = dbService.ReadTrackById(ids);

			// Order by Hamming Similarity
			// Using PLINQ
			//OrderedParallelQuery<KeyValuePair<int, QueryStats>> order = allCandidates.AsParallel()
			IOrderedEnumerable<KeyValuePair<int, QueryStats>> order = allCandidates
				.OrderBy((pair) => pair.Value.OrderingValue =
				         pair.Value.HammingDistance / pair.Value.NumberOfTotalTableVotes
				         + 0.4 * pair.Value.MinHammingDistance);
			
			// Join on the ID properties.
			var joined = from o in order
				join track in tracks on o.Key equals track.Id
				select new { track, o.Value.Similarity };

			Dictionary<Track, double> stats = joined.ToDictionary(Key => Key.track, Value => Value.Similarity);
			
			Dbg.WriteLine ("Find Similar From Audio Samples - Total Execution Time: {0} ms", t.Stop().TotalMilliseconds);
			return stats;
		}
		
		/// <summary>
		/// Find Similar Tracks using passed audio samples as input and return a List
		/// </summary>
		/// <param name="lshHashTables">Number of hash tables from the database</param>
		/// <param name="lshGroupsPerKey">Number of groups per hash table</param>
		/// <param name="thresholdTables">Minimum number of hash tables that must be found for one signature to be considered a candidate (0 and 1 = return all candidates, 2+ = return only exact matches)</param>
		/// <param name="param">Audio File Work Unit Parameter Object</param>
		/// <param name="optimizeSignatureCount">Reduce the number of signatures in order to increase the search performance</param>
		/// <param name="doSearchEverything">disregard the local sensitivity hashes and search the whole database</param>
		/// <param name="splashScreen">The "please wait" splash screen (or null)</param>
		/// <returns>a list of perceptually similar tracks</returns>
		public List<QueryResult> FindSimilarFromAudioSamplesList(
			int lshHashTables,
			int lshGroupsPerKey,
			int thresholdTables,
			WorkUnitParameterObject param,
			bool optimizeSignatureCount,
			bool doSearchEverything,
			SplashSceenWaitingForm splashScreen) {
			
			var t = new DbgTimer();
			t.Start ();

			if (splashScreen != null) splashScreen.SetProgress(2, "Creating fingerprints from audio samples ...");
			
			// Get fingerprints
			double[][] logSpectrogram;
			List<bool[]> fingerprints = fingerprintService.CreateFingerprintsFromAudioSamples(param.AudioSamples, param, out logSpectrogram);

			#if DEBUG
			// Save debug images using fingerprinting methods
			//Analyzer.SaveFingerprintingDebugImages(param.FileName, logSpectrogram, fingerprints, fingerprintService, param.FingerprintingConfiguration);
			#endif
			
			if (splashScreen != null) splashScreen.SetProgress(3, String.Format("Successfully created {0} fingerprints.", fingerprints.Count));
			
			// If the number of signatures is to big, only keep the first MAX_SIGNATURE_COUNT to avoid a very time consuming search
			if (optimizeSignatureCount && fingerprints.Count > MAX_SIGNATURE_COUNT) {
				if (splashScreen != null) splashScreen.SetProgress(4, String.Format("Only using the first {0} fingerprints out of {1}.", MAX_SIGNATURE_COUNT, fingerprints.Count));
				fingerprints.RemoveRange(MAX_SIGNATURE_COUNT, fingerprints.Count - MAX_SIGNATURE_COUNT);
				Dbg.WriteLine("Only using the first {0} fingerprints.", MAX_SIGNATURE_COUNT);
			}
			
			long elapsedMiliseconds = 0;
			
			// Query the database using Min Hash
			Dictionary<int, QueryStats> allCandidates = QueryFingerprintManager.QueryOneSongMinHash(
				fingerprints,
				dbService,
				minHash,
				lshHashTables,
				lshGroupsPerKey,
				thresholdTables,
				ref elapsedMiliseconds,
				doSearchEverything,
				splashScreen);

			if (splashScreen != null) splashScreen.SetProgress(91, String.Format("Found {0} candidates.", allCandidates.Count));
			
			IEnumerable<int> ids = allCandidates.Select(p => p.Key);
			IList<Track> tracks = dbService.ReadTrackById(ids);

			if (splashScreen != null) splashScreen.SetProgress(95, String.Format("Reading {0} tracks.", tracks.Count));
			
			// Order by Hamming Similarity
			// TODO: What does the 0.4 number do here?
			// there doesn't seem to be any change using another number?!

			/*
			// Using PLINQ
			IOrderedEnumerable<KeyValuePair<int, QueryStats>> order = allCandidates
				.OrderBy((pair) => pair.Value.OrderingValue =
				         pair.Value.HammingDistance / pair.Value.NumberOfTotalTableVotes
				         + 0.4 * pair.Value.MinHammingDistance);


			var fingerprintList = (from o in order
			                       join track in tracks on o.Key equals track.Id
			                       select new FindSimilar.QueryResult {
			                       	Id = track.Id,
			                       	Path = track.FilePath,
			                       	Duration = track.TrackLengthMs,
			                       	Similarity = o.Value.Similarity
			                       }).ToList();
			 */
			
			// http://msdn.microsoft.com/en-us/library/dd460719(v=vs.110).aspx
			// http://stackoverflow.com/questions/2767709/c-sharp-joins-where-with-linq-and-lambda
			// Lambda query to order the candidates
			var order = allCandidates.AsParallel()
				.OrderBy((pair) => pair.Value.OrderingValue =
				         pair.Value.HammingDistance / pair.Value.NumberOfTotalTableVotes
				         + 0.4 * pair.Value.MinHammingDistance)
				.Take(200);
			
			// TODO: Be able to create the above query as a LINQ query
			
			// Join on the ID properties.
			var fingerprintList = (from o in order.AsParallel()
			                       join track in tracks.AsParallel() on o.Key equals track.Id
			                       select new QueryResult {
			                       	Id = track.Id,
			                       	Path = track.FilePath,
			                       	Duration = track.TrackLengthMs,
			                       	Similarity = o.Value.Similarity
			                       })
				.OrderByDescending((ord) => ord.Similarity)
				.ToList();
			
			if (splashScreen != null) splashScreen.SetProgress(100, "Ready!");
			
			Dbg.WriteLine ("FindSimilarFromAudioSamplesList - Total Execution Time: {0} ms", t.Stop().TotalMilliseconds);
			return fingerprintList;
		}
		
		/// <summary>
		/// Insert track into database
		/// </summary>
		/// <param name="track">Track</param>
		/// <param name="hashTables">Number of hash tables (e.g. 25)</param>
		/// <param name="hashKeys">Number of hash keys (e.g. 4)</param>
		/// <param name="param">WorkUnitParameterObject parameters</param>
		/// <param name="logSpectrogram">Return the log spectrogram in an out parameter</param>
		/// <param name="fingerprints">Return the fingerprints in an out parameter</param>
		/// <returns></returns>
		public bool InsertTrackInDatabaseUsingSamples(Track track, int hashTables, int hashKeys, WorkUnitParameterObject param, out double[][] logSpectrogram, out List<bool[]> fingerprints)
		{
			// TODO: wrap all of this within one transaction
			if (dbService.InsertTrack(track)) {

				// return both logSpectrogram and fingerprints in the out variables
				fingerprints = fingerprintService.CreateFingerprintsFromAudioSamples(param.AudioSamples, param, out logSpectrogram);
				
				List<Fingerprint> inserted = AssociateFingerprintsToTrack(fingerprints, track.Id);
				if (dbService.InsertFingerprint(inserted)) {
					return HashFingerprintsUsingMinHash(inserted, track, hashTables, hashKeys);
				} else {
					return false;
				}
			} else {
				logSpectrogram = null;
				fingerprints = null;
				return false;
			}
		}
		
		/// <summary>
		/// Associate fingerprint signatures with a specific track
		/// </summary>
		/// <param name="fingerprintSignatures">Signatures built from one track</param>
		/// <param name="trackId">Track id, which is the parent for this fingerprints</param>
		/// <returns>List of fingerprint entity objects</returns>
		private List<Fingerprint> AssociateFingerprintsToTrack(IEnumerable<bool[]> fingerprintSignatures, int trackId)
		{
			const int fakeId = -1;
			var fingers = new List<Fingerprint>();
			int c = 0;
			foreach (bool[] signature in fingerprintSignatures)
			{
				fingers.Add(new Fingerprint(fakeId, signature, trackId, c));
				c++;
			}
			return fingers;
		}
		
		/// <summary>
		/// Hash Fingerprints using Min-Hash algorithm
		/// </summary>
		/// <param name="listOfFingerprintsToHash">List of fingerprints already inserted in the database</param>
		/// <param name="track">Track of the corresponding fingerprints</param>
		/// <param name="hashTables">Number of hash tables (e.g. 25)</param>
		/// <param name="hashKeys">Number of hash keys (e.g. 4)</param>
		private bool HashFingerprintsUsingMinHash(IEnumerable<Fingerprint> listOfFingerprintsToHash, Track track, int hashTables, int hashKeys)
		{
			const int fakeId = -1;
			var listToInsert = new List<HashBinMinHash>();
			foreach (Fingerprint fingerprint in listOfFingerprintsToHash)
			{
				int[] hashBins = minHash.ComputeMinHashSignature(fingerprint.Signature); //Compute Min Hashes
				Dictionary<int, long> hashTable = minHash.GroupMinHashToLSHBuckets(hashBins, hashTables, hashKeys);
				foreach (KeyValuePair<int, long> item in hashTable)
				{
					var hash = new HashBinMinHash(fakeId, item.Value, item.Key, track.Id, fingerprint.Id);
					listToInsert.Add(hash);
				}
			}
			return dbService.InsertHashBin(listToInsert);
		}
	}
}