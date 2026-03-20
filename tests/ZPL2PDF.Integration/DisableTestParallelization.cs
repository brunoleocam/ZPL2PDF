using Xunit;

// Prevent env-var races between daemon background processes.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

