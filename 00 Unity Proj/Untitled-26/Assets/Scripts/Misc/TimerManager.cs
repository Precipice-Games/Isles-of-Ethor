using System.Collections.Generic; // Used for List.
using UnityEngine; // TimerManager derives from MonoBehaviour.

// I added this script that was created by GitHub user "The Shroom" to the project.
// It is from 8+ years ago so it may be out of date in some ways, but I wanted to
// have something to work with when I was creating the PuzzleFeedback.cs script.
// -- Nikki

// This simple timer system allows you to create timers that can invoke
// delegates when they have finished counting down. It also allows you to pause,
// unpause, reset, and stop the timers as well as set delegates to invoke when the
// timer ticks (every frame) and when it finishes.

// The author created the timers to be updated within Update(). The TimerManager
// class also handles the creation and destruction of all the created timers.

// Note: You should always call DeleteTimer() on a timer when you are done with it,
// otherwise it will stay in memory forever which will cause a memory leak.

namespace SimpleTimer // Project name
{
    /// <summary>
    /// Handles creation and destruction of all the created timers as well as updates them each frame.
    /// </summary>
	public class TimerManager : MonoBehaviour // The only reason we derive from MonoBehaviours to get access to the Update function. (probably a bad idea)
	{
        /// <summary>
        /// A timer which invokes a delegate when it has finished counting down.
        /// This should not be instantiated directly, use TimerManager.CreateTimer() instead.
        /// </summary>
	    public class Timer
	    {
            /// <summary>
            /// Delegate type that onTimerFinished uses.
            /// </summary>
			public delegate void OnTimerFinished();

            /// <summary>
            /// Gets invoked when the timer has finished counting down. This is set by the user.
            /// </summary>
			private OnTimerFinished onTimerFinished;

			/// <summary>
			/// Delegate type that onTimerTicked uses.true
			/// </summary>
			public delegate void OnTimerTicked();

			/// <summary>
			/// Gets invoked every time the timer ticks. (once every frame)
			/// </summary>
			public OnTimerTicked onTimerTicked;

            /// <summary>
            /// The time that has elapsed since the timer started counting. (in seconds)
            /// </summary>
			private float elapsedTime;

            /// <summary>
            /// The amount of time (in seconds) the timer should wait before finishing and invoking.
            /// </summary>
			private float waitTime;

            /// <summary>
            /// If true, the timer is paused and will not count down.
            /// </summary>
			private bool paused;

            /// <summary>
            /// If true, the timer has finished.
            /// </summary>
			private bool finished;

            /// <summary>
            /// Creates and initializes a new instance of a Timer.
            /// </summary>
			internal Timer(float waitTime, Timer.OnTimerFinished onTimerFinished = null, Timer.OnTimerTicked onTimerTicked = null, bool startNow = true)
			{
		    	SetWaitTime(waitTime);
		    	this.onTimerFinished = onTimerFinished;
				this.onTimerTicked = onTimerTicked;
				
		    	if (startNow)
		    	{
					Start();
		    	}
		    	else
		    	{
					Pause();
		    	}
			}

			/// <summary>
			/// Starts the timer. Has the same functionality as Pause().
			/// </summary>
			public void Start()
			{
		    	Debug.Log("Starting SimpleTimer!"); // TODO (The Shroom): Implement the actual timers.
			}

			/// <summary>
			/// Pauses and resets the timer.
			/// </summary>
			public void Stop()
	        {
		   		Pause();
		    	Reset();
			}

			/// <summary>
			/// Resets the counter of the timer.
			/// </summary>
			public void Reset()
			{
                finished = false;
		    	SetElapsedTime(0.0f);
			}

			/// <summary>
			/// Pauses the timer and prevents it from counting.
			/// </summary>
			public void Pause()
			{
			    paused = true;
			}

			/// <summary>
			/// Unpauses the timer so it starts counting again.
			/// </summary>
			public void Unpause()
			{
			    paused = false;
			}

			/// <summary>
			/// Sets the current value of the timer.
			/// </summary>
			public void SetElapsedTime(float newTime)
			{
			    elapsedTime = newTime;
			}

			/// <summary>
			/// Returns the current value of the timer.
			/// </summary>
			public float GetElapsedTime()
			{
			    return elapsedTime;
			}

			/// <summary>
			/// Sets the amount of seconds the timer will wait before finishing.
			/// </summary>
			public void SetWaitTime(float newTime)
			{
		   		waitTime = newTime;
			}

			/// <summary>
			/// Returns the amount of seconds the timer will wait before finishing.
			/// </summary>
			public float GetWaitTime()
			{
		    	return waitTime;
			}

			/// <summary>
			/// Sets the delegate to invoke when the timer finishes.
            /// When the timer is finished and/or you don't need it anymore, don't forget to call RemoveListener.
			/// </summary>
			public void AddListener(OnTimerFinished delgt)
			{
		    	onTimerFinished += delgt;
			}

			/// <summary>
			/// Sets the delegate to invoke when the timer ticks.
			/// When the timer has finished and/or you don't need it anymore, don't forget to call RemoveListener.
			/// </summary>
			public void AddListener(OnTimerTicked delgt)
			{
				onTimerTicked += delgt;
			}

            /// <summary>
            /// Removes the specified delegate from the delegate invokation list.
            /// This should be called in cases where you want to unsubscribe a function.
            /// </summary>
            public void RemoveListener(OnTimerFinished delgt)
            {
                onTimerFinished -= delgt;
            }

			/// <summary>
			/// Removes the specified delegate from the delegate invokation list.
			/// This should be called in cases where you want to unsubscribe a function.
			public void RemoveListener(OnTimerTicked delgt)
			{
				onTimerTicked -= delgt;
			}

            /// <summary>
            /// Returns true if the timer is paused, false otherwise.
            /// </summary>
            public bool IsPaused()
            {
                return paused;
            }

            /// <summary>
            /// Returns true if the timer has finished counting down, false otherwise.
            /// </summary>
            public bool IsFinished()
            {
                return finished;
            }

            /// <summary>
            /// Handles the countdown of the timer as well as calls the Finish() function when the timer has counted down.
            /// </summary>
			internal void Update()
			{
		    	if (!paused && !finished) // If the timer isn't paused and it hasn't finished,
		    	{
                    // we can go ahead and count up. (This is also referred to as the countdown, even though it's not a literal countdown)
					elapsedTime += Time.deltaTime;
					InvokeTickedDelegate();
		    	}

                // If the timer hasn't finished already and the elapsed time is the same or has exceeded the waiting time,
		    	if (!finished && elapsedTime >= waitTime)
		   		{
					Finish(); // we can go ahead and finish the timer.
		    	}
			}

            /// <summary>
            /// Gets called when the timer has finished its countdown. Sets the state of the timer and called the invoke function.
            /// </summary>
			private void Finish()
			{
			    finished = true;
			    InvokeFinishedDelegate(); // The timer has finished; invoke the delegate.
			}

            /// <summary>
            /// Gets called when the timer has finished its countdown. Handles invoking the onTimerFinished delegate.
            /// </summary>
			private void InvokeFinishedDelegate()
			{
			    if (onTimerFinished != null) // Only invoke if it has a subscriber.
			    {
			        onTimerFinished.Invoke();
			    }
			}

			/// <summary>
			/// Gets called when the timer ticks (and it isn't paused). Handles invoking the onTimerTicked delegate.
			/// </summary>
			private void InvokeTickedDelegate()
			{
				if (onTimerTicked != null)
				{
					onTimerTicked.Invoke();
				}
			}
	    }

        /// <summary>
        /// A list of all created timers. This is the reason to why not calling DeleteTimer on a created timer causes a memory leak.
        /// </summary>
	    private static List<Timer> timers = new List<Timer>();

        /// <summary>
       	/// Creates a new timer. If startNow is set to true, it will automatically start the countdown. If not you will have to call Start().
		/// If you don't want to use one of the delegates (for example the onTimerFinished) you can just pass null to it.
        /// </summary>
	    public static Timer CreateTimer(float waitTime, Timer.OnTimerFinished onTimerFinished = null, Timer.OnTimerTicked onTimerTicked = null, bool startNow = true)
	    {
	        Debug.Log("Creating SimpleTimer");
			timers.Add(new Timer(waitTime, onTimerFinished, onTimerTicked, startNow));
			return timers[timers.Count - 1]; // Return a reference to the created timer.
	    }

        /// <summary>
        /// Deletes the timer. This should ALWAYS be called when the timer is no longer used.
        /// If you never call it the timer is gonna stay in memory forever which mean that you'll get a memory leak.
        /// Note: Still trying to find a better, automatic way of removing them. At first I removed them as soon as it finished,
        ///       but if the user is trying to reset the timer and restart it after it has finished this wouldn't be possible.
        /// </summary>
        public static void DeleteTimer(Timer timer)
        {
            timers.Remove(timer);
        }

        /// <summary>
        /// Loops through all created timers and updates them.
        /// </summary>
	    private void Update()
	    {
		    //Debug.Log("Update was called on a timer.");
		    
		    // Had to comment this out because it was throwing an error
			// foreach (Timer t in timers) // Call update all timers so that they can add deltatime each frame.
			// {
			// 	t.Update();
			// }
			
			for (int i = timers.Count - 1; i >= 0; i--)
			{
				timers[i].Update();
			}
	    }
	}
}