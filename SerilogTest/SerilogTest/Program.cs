using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SerilogTest
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information() // The MinimumLevel configuration object provides for one of the log event levels to be specified as the minimum. In the example above, log events with level Debug and higher will be processed and ultimately written to the console.
                .Enrich.With(new ThreadIdEnricher()) // See the Thread Enricher class below for details on log enrichment.
                .WriteTo.LiterateConsole( // The LiterateConsole is a Sink. Log event sinks generally record log events to some external representation, typically the console, a file or data store. Serilog sinks are distributed via NuGet.
                    outputTemplate: "{Timestamp:HH:MM} [{Level}] ({ThreadId}) {Message} {NewLine}{Exception}" // Here I've set a custom output template. Comment this line out if you'd like to see the default template.
                    )
                .CreateLogger();

            // Minimum level

            // Level	    Usage

            // Verbose	    Verbose is the noisiest level, rarely (if ever) enabled for a production app.
            // Debug	    Debug is used for internal system events that are not necessarily observable from the outside, but useful when determining how something happened.
            // Information	Information events describe things happening in the system that correspond to its responsibilities and functions. Generally these are the observable actions the system can perform.
            // Warning	    When service is degraded, endangered, or may be behaving outside of its expected parameters, Warning level events are used.
            // Error	    When functionality is unavailable or expectations broken, an Error event is used.
            // Fatal	    The most critical level, Fatal events demand immediate attention.

            Log.Information("This is an Information level log entry. It should be shown.");
            Log.Error("This is an Error level log entry. It should be shown.");
            Log.Debug("This is a Debug level log entry. It should not be shown.");
            
            // Test passing in an exception to the logger. 
            try
            {
                Log.Information("This time we're going to add an exception to an Error log.");
                throw new DivideByZeroException();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "We caught an exception!");
            }

            // Test the enricher function, which gives us thread id, by making use of multiple threads.
            try
            {
                var stringList = new List<string>() { "Chase", "Kevin", "Thomas", "Beth", "Rachel" };

                Parallel.ForEach(stringList, (str) =>
                {
                    Log.Information(str);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "We caught an exception!");
            }

            Console.ReadKey();
        }
    }

    public class ThreadIdEnricher : ILogEventEnricher
    {
        // Enrichers are simple components that add, remove or modify the properties attached to a log event.
        // This can be used for the purpose of attaching a thread id to each event, for example.

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ThreadId", Thread.CurrentThread.ManagedThreadId));
        }
    }
}