using System.Collections.Generic;
using System;
using UnityEngine;

namespace IuvoTiming
{
    public class Timer
    {
        struct TimerAction
        {
            public enum Timer_Activity_Mode
            {
                LONG_TERM_COUNTDOW, SHORT_COUNTDOWN, COUNTDOWN,
                LONG_TERM_TIMER, SHORT_TERM_TIMER, TIMER,
                LONG_TERM_STOPWATCH, SHORT_TERM_STOPWATCH, STOPWATCH,
                SPECIAL_ACTIVITY, NULL
            }

            public Timer_Activity_Mode ActivityMode;

            public TimerAction(Timer_Activity_Mode mode)
            {
                ActivityMode = mode;
            }
        }


        TimerAction action;
        float startOfTimerLife = 0.0f;
        float currentTime = 0.0f;
        float targetTime = 0.0f;
        public bool timerFinished = false;
        public bool timerRunning = false;
        public bool timerPaused = false;

        public void StartTimer()
        {
            if (!timerRunning)
                timerRunning = true;
        }

        public void PauseTimer()
        {
            if (timerRunning)
            {
                timerPaused = true;
                timerRunning = false;
            }
        }

        public void UnpauseTimer()
        {
            if (timerPaused)
            {
                timerPaused = false;
                timerRunning = true;
            }
        }

        public void ResetTimer(float newTargetTime)
        {
            timerRunning = false;
            currentTime = 0.0f;
            targetTime = newTargetTime;
        }


        void Update(float passageOfTime)
        {
            if (timerFinished)
            {
                return;
            }
            else
            {
                if (!timerPaused && timerRunning)
                {
                    currentTime += Time.deltaTime;
                }

                if (currentTime >= targetTime)
                {
                    timerFinished = true;
                    timerRunning = false;
                }
            }
        }

        void FixedUpdate(float progressionOfTime)
        {


        }
    }
}