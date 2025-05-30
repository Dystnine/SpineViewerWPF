/******************************************************************************
 * Spine Runtimes Software License
 * Version 2.1
 * 
 * Copyright (c) 2013, Esoteric Software
 * All rights reserved.
 * 
 * You are granted a perpetual, non-exclusive, non-sublicensable and
 * non-transferable license to install, execute and perform the Spine Runtimes
 * Software (the "Software") solely for internal use. Without the written
 * permission of Esoteric Software (typically granted by licensing Spine), you
 * may not (a) modify, translate, adapt or otherwise create derivative works,
 * improvements of the Software or develop new applications using the Software
 * or (b) remove, delete, alter or obscure any trademarks or any copyright,
 * trademark, patent or other intellectual property or proprietary rights
 * notices on or in the Software, including any copy thereof. Redistributions
 * in binary or source form must include this license and terms.
 * 
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Spine2_1_25
{
    public class AnimationState
    {
        private AnimationStateData data;
        private List<TrackEntry> tracks = new List<TrackEntry>();
        private List<Event> events = new List<Event>();
        private float timeScale = 1;

        public AnimationStateData Data { get { return data; } }
        public float TimeScale { get { return timeScale; } set { timeScale = value; } }

        public delegate void StartEndDelegate(AnimationState state, int trackIndex);
        public event StartEndDelegate Start;
        public event StartEndDelegate End;

        public delegate void EventDelegate(AnimationState state, int trackIndex, Event e);
        public event EventDelegate Event;

        public delegate void CompleteDelegate(AnimationState state, int trackIndex, int loopCount);
        public event CompleteDelegate Complete;

        public AnimationState(AnimationStateData data)
        {
            if (data == null) throw new ArgumentNullException("data cannot be null.");
            this.data = data;
        }

        public void Update(float delta)
        {
            delta *= timeScale;
            for (int i = 0; i < tracks.Count; i++)
            {
                TrackEntry current = tracks[i];
                if (current == null) continue;

                float trackDelta = delta * current.timeScale;
                float time = current.time + trackDelta;
                float endTime = current.endTime;

                current.time = time;
                if (current.previous != null)
                {
                    current.previous.time += trackDelta;
                    current.mixTime += trackDelta;
                }

                // Check if completed the animation or a loop iteration.
                if (current.loop ? (current.lastTime % endTime > time % endTime) : (current.lastTime < endTime && time >= endTime))
                {
                    int count = (int)(time / endTime);
                    current.OnComplete(this, i, count);
                    if (Complete != null) Complete(this, i, count);
                }

                TrackEntry next = current.next;
                if (next != null)
                {
                    next.time = current.lastTime - next.delay;
                    if (next.time >= 0) SetCurrent(i, next);
                }
                else
                {
                    // End non-looping animation when it reaches its end time and there is no next entry.
                    if (!current.loop && current.lastTime >= current.endTime) ClearTrack(i);
                }
            }
        }

        public void Apply(Skeleton skeleton)
        {
            List<Event> events = this.events;

            for (int i = 0; i < tracks.Count; i++)
            {
                TrackEntry current = tracks[i];
                if (current == null) continue;

                events.Clear();

                float time = current.time;
                bool loop = current.loop;
                if (!loop && time > current.endTime) time = current.endTime;

                TrackEntry previous = current.previous;
                if (previous == null)
                {
                    if (current.mix == 1)
                        current.animation.Apply(skeleton, current.lastTime, time, loop, events);
                    else
                        current.animation.Mix(skeleton, current.lastTime, time, loop, events, current.mix);
                }
                else
                {
                    float previousTime = previous.time;
                    if (!previous.loop && previousTime > previous.endTime) previousTime = previous.endTime;
                    previous.animation.Apply(skeleton, previousTime, previousTime, previous.loop, null);

                    float alpha = current.mixTime / current.mixDuration * current.mix;
                    if (alpha >= 1)
                    {
                        alpha = 1;
                        current.previous = null;
                    }
                    current.animation.Mix(skeleton, current.lastTime, time, loop, events, alpha);
                }

                for (int ii = 0, nn = events.Count; ii < nn; ii++)
                {
                    Event e = events[ii];
                    current.OnEvent(this, i, e);
                    if (Event != null) Event(this, i, e);
                }

                current.lastTime = current.time;
            }
        }

        public void ClearTracks()
        {
            for (int i = 0, n = tracks.Count; i < n; i++)
                ClearTrack(i);
            tracks.Clear();
        }

        public void ClearTrack(int trackIndex)
        {
            if (trackIndex >= tracks.Count) return;
            TrackEntry current = tracks[trackIndex];
            if (current == null) return;

            current.OnEnd(this, trackIndex);
            if (End != null) End(this, trackIndex);

            tracks[trackIndex] = null;
        }

        private TrackEntry ExpandToIndex(int index)
        {
            if (index < tracks.Count) return tracks[index];
            while (index >= tracks.Count)
                tracks.Add(null);
            return null;
        }

        private void SetCurrent(int index, TrackEntry entry)
        {
            TrackEntry current = ExpandToIndex(index);
            if (current != null)
            {
                TrackEntry previous = current.previous;
                current.previous = null;

                current.OnEnd(this, index);
                if (End != null) End(this, index);

                entry.mixDuration = data.GetMix(current.animation, entry.animation);
                if (entry.mixDuration > 0)
                {
                    entry.mixTime = 0;
                    // If a mix is in progress, mix from the closest animation.
                    if (previous != null && current.mixTime / current.mixDuration < 0.5f)
                        entry.previous = previous;
                    else
                        entry.previous = current;
                }
            }

            tracks[index] = entry;

            entry.OnStart(this, index);
            if (Start != null) Start(this, index);
        }

        public TrackEntry SetAnimation(int trackIndex, String animationName, bool loop)
        {
            Animation animation = data.skeletonData.FindAnimation(animationName);
            if (animation == null) throw new ArgumentException("Animation not found: " + animationName);
            return SetAnimation(trackIndex, animation, loop);
        }

        /// <summary>Set the current animation. Any queued animations are cleared.</summary>
        public TrackEntry SetAnimation(int trackIndex, Animation animation, bool loop)
        {
            if (animation == null) throw new ArgumentException("animation cannot be null.");
            TrackEntry entry = new TrackEntry();
            entry.animation = animation;
            entry.loop = loop;
            entry.time = 0;
            entry.endTime = animation.Duration;
            SetCurrent(trackIndex, entry);
            return entry;
        }

        public TrackEntry AddAnimation(int trackIndex, String animationName, bool loop, float delay)
        {
            Animation animation = data.skeletonData.FindAnimation(animationName);
            if (animation == null) throw new ArgumentException("Animation not found: " + animationName);
            return AddAnimation(trackIndex, animation, loop, delay);
        }

        /// <summary>Adds an animation to be played delay seconds after the current or last queued animation.</summary>
        /// <param name="delay">May be <= 0 to use duration of previous animation minus any mix duration plus the negative delay.</param>
        public TrackEntry AddAnimation(int trackIndex, Animation animation, bool loop, float delay)
        {
            if (animation == null) throw new ArgumentException("animation cannot be null.");
            TrackEntry entry = new TrackEntry();
            entry.animation = animation;
            entry.loop = loop;
            entry.time = 0;
            entry.endTime = animation.Duration;

            TrackEntry last = ExpandToIndex(trackIndex);
            if (last != null)
            {
                while (last.next != null)
                    last = last.next;
                last.next = entry;
            }
            else
                tracks[trackIndex] = entry;

            if (delay <= 0)
            {
                if (last != null)
                    delay += last.endTime - data.GetMix(last.animation, animation);
                else
                    delay = 0;
            }
            entry.delay = delay;

            return entry;
        }

        /// <returns>May be null.</returns>
        public TrackEntry GetCurrent(int trackIndex)
        {
            if (trackIndex >= tracks.Count) return null;
            return tracks[trackIndex];
        }

        override public String ToString()
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0, n = tracks.Count; i < n; i++)
            {
                TrackEntry entry = tracks[i];
                if (entry == null) continue;
                if (buffer.Length > 0) buffer.Append(", ");
                buffer.Append(entry.ToString());
            }
            if (buffer.Length == 0) return "<none>";
            return buffer.ToString();
        }
    }

    public class TrackEntry
    {
        internal TrackEntry next, previous;
        internal Animation animation;
        internal bool loop;
        internal float delay, time, lastTime = -1, endTime, timeScale = 1;
        internal float mixTime, mixDuration, mix = 1;

        public Animation Animation { get { return animation; } }
        public float Delay { get { return delay; } set { delay = value; } }
        public float Time { get { return time; } set { time = value; } }
        public float LastTime { get { return lastTime; } set { lastTime = value; } }
        public float EndTime { get { return endTime; } set { endTime = value; } }
        public float TimeScale { get { return timeScale; } set { timeScale = value; } }
        public float Mix { get { return mix; } set { mix = value; } }
        public bool Loop { get { return loop; } set { loop = value; } }

        public event AnimationState.StartEndDelegate Start;
        public event AnimationState.StartEndDelegate End;
        public event AnimationState.EventDelegate Event;
        public event AnimationState.CompleteDelegate Complete;

        internal void OnStart(AnimationState state, int index)
        {
            if (Start != null) Start(state, index);
        }

        internal void OnEnd(AnimationState state, int index)
        {
            if (End != null) End(state, index);
        }

        internal void OnEvent(AnimationState state, int index, Event e)
        {
            if (Event != null) Event(state, index, e);
        }

        internal void OnComplete(AnimationState state, int index, int loopCount)
        {
            if (Complete != null) Complete(state, index, loopCount);
        }

        override public String ToString()
        {
            return animation == null ? "<none>" : animation.name;
        }
    }
}
