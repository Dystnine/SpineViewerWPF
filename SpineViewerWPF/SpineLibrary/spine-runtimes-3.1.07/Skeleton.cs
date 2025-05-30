/******************************************************************************
 * Spine Runtimes Software License
 * Version 2.3
 * 
 * Copyright (c) 2013-2015, Esoteric Software
 * All rights reserved.
 * 
 * You are granted a perpetual, non-exclusive, non-sublicensable and
 * non-transferable license to use, install, execute and perform the Spine
 * Runtimes Software (the "Software") and derivative works solely for personal
 * or internal use. Without the written permission of Esoteric Software (see
 * Section 2 of the Spine Software License Agreement), you may not (a) modify,
 * translate, adapt or otherwise create derivative works, improvements of the
 * Software or develop new applications using the Software or (b) remove,
 * delete, alter or obscure any trademarks or any copyright, trademark, patent
 * or other intellectual property or proprietary rights notices on or in the
 * Software, including any copy thereof. Redistributions in binary or source
 * form must include this license and terms.
 * 
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;

namespace Spine3_1_07
{
    public class Skeleton
    {
        internal SkeletonData data;
        internal ExposedList<Bone> bones;
        internal ExposedList<Slot> slots;
        internal ExposedList<Slot> drawOrder;
        internal ExposedList<IkConstraint> ikConstraints;
        internal ExposedList<TransformConstraint> transformConstraints;
        private ExposedList<IUpdatable> updateCache = new ExposedList<IUpdatable>();
        internal Skin skin;
        internal float r = 1, g = 1, b = 1, a = 1;
        internal float time;
        internal bool flipX, flipY;
        internal float x, y;

        public SkeletonData Data { get { return data; } }
        public ExposedList<Bone> Bones { get { return bones; } }
        public ExposedList<Slot> Slots { get { return slots; } }
        public ExposedList<Slot> DrawOrder { get { return drawOrder; } }
        public ExposedList<IkConstraint> IkConstraints { get { return ikConstraints; } set { ikConstraints = value; } }
        public Skin Skin { get { return skin; } set { skin = value; } }
        public float R { get { return r; } set { r = value; } }
        public float G { get { return g; } set { g = value; } }
        public float B { get { return b; } set { b = value; } }
        public float A { get { return a; } set { a = value; } }
        public float Time { get { return time; } set { time = value; } }
        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public bool FlipX { get { return flipX; } set { flipX = value; } }
        public bool FlipY { get { return flipY; } set { flipY = value; } }

        public Bone RootBone
        {
            get
            {
                return bones.Count == 0 ? null : bones.Items[0];
            }
        }

        public Skeleton(SkeletonData data)
        {
            if (data == null) throw new ArgumentNullException("data cannot be null.");
            this.data = data;

            bones = new ExposedList<Bone>(data.bones.Count);
            foreach (BoneData boneData in data.bones)
            {
                Bone parent = boneData.parent == null ? null : bones.Items[data.bones.IndexOf(boneData.parent)];
                Bone bone = new Bone(boneData, this, parent);
                if (parent != null) parent.children.Add(bone);
                bones.Add(bone);
            }

            slots = new ExposedList<Slot>(data.slots.Count);
            drawOrder = new ExposedList<Slot>(data.slots.Count);
            foreach (SlotData slotData in data.slots)
            {
                Bone bone = bones.Items[data.bones.IndexOf(slotData.boneData)];
                Slot slot = new Slot(slotData, bone);
                slots.Add(slot);
                drawOrder.Add(slot);
            }

            ikConstraints = new ExposedList<IkConstraint>(data.ikConstraints.Count);
            foreach (IkConstraintData ikConstraintData in data.ikConstraints)
                ikConstraints.Add(new IkConstraint(ikConstraintData, this));

            transformConstraints = new ExposedList<TransformConstraint>(data.transformConstraints.Count);
            foreach (TransformConstraintData transformConstraintData in data.transformConstraints)
                transformConstraints.Add(new TransformConstraint(transformConstraintData, this));

            UpdateCache();
            UpdateWorldTransform();
        }

        /// <summary>Caches information about bones and constraints. Must be called if bones or constraints are added
        /// or removed.</summary>
        public void UpdateCache()
        {
            ExposedList<Bone> bones = this.bones;
            ExposedList<IUpdatable> updateCache = this.updateCache;
            ExposedList<IkConstraint> ikConstraints = this.ikConstraints;
            ExposedList<TransformConstraint> transformConstraints = this.transformConstraints;
            int ikConstraintsCount = ikConstraints.Count;
            int transformConstraintsCount = transformConstraints.Count;
            updateCache.Clear();
            for (int i = 0, n = bones.Count; i < n; i++)
            {
                Bone bone = bones.Items[i];
                updateCache.Add(bone);
                for (int ii = 0; ii < ikConstraintsCount; ii++)
                {
                    IkConstraint ikConstraint = ikConstraints.Items[ii];
                    if (bone == ikConstraint.bones.Items[ikConstraint.bones.Count - 1])
                    {
                        updateCache.Add(ikConstraint);
                        break;
                    }
                }
            }

            for (int i = 0; i < transformConstraintsCount; i++)
            {
                TransformConstraint transformConstraint = transformConstraints.Items[i];
                for (int ii = updateCache.Count - 1; i >= 0; ii--)
                {
                    IUpdatable updateable = updateCache.Items[ii];
                    if (updateable == transformConstraint.bone || updateable == transformConstraint.target)
                    {
                        updateCache.Insert(ii + 1, transformConstraint);
                        break;
                    }
                }
            }
        }

        /// <summary>Updates the world transform for each bone and applies constraints.</summary>
        public void UpdateWorldTransform()
        {
            ExposedList<IUpdatable> updateCache = this.updateCache;
            for (int i = 0, n = updateCache.Count; i < n; i++)
                updateCache.Items[i].Update();
        }

        /// <summary>Sets the bones, constraints, and slots to their setup pose values.</summary>
        public void SetToSetupPose()
        {
            SetBonesToSetupPose();
            SetSlotsToSetupPose();
        }

        /// <summary>Sets the bones and constraints to their setup pose values.</summary>
        public void SetBonesToSetupPose()
        {
            ExposedList<Bone> bones = this.bones;
            for (int i = 0, n = bones.Count; i < n; i++)
                bones.Items[i].SetToSetupPose();

            ExposedList<IkConstraint> ikConstraints = this.ikConstraints;
            for (int i = 0, n = ikConstraints.Count; i < n; i++)
            {
                IkConstraint constraint = ikConstraints.Items[i];
                constraint.bendDirection = constraint.data.bendDirection;
                constraint.mix = constraint.data.mix;
            }

            ExposedList<TransformConstraint> transformConstraints = this.transformConstraints;
            for (int i = 0, n = transformConstraints.Count; i < n; i++)
            {
                TransformConstraint constraint = transformConstraints.Items[i];
                constraint.translateMix = constraint.data.translateMix;
                constraint.x = constraint.data.x;
                constraint.y = constraint.data.y;
            }
        }

        public void SetSlotsToSetupPose()
        {
            ExposedList<Slot> slots = this.slots;
            drawOrder.Clear();
            for (int i = 0, n = slots.Count; i < n; i++)
                drawOrder.Add(slots.Items[i]);

            for (int i = 0, n = slots.Count; i < n; i++)
                slots.Items[i].SetToSetupPose(i);
        }

        /// <returns>May be null.</returns>
        public Bone FindBone(String boneName)
        {
            if (boneName == null) throw new ArgumentNullException("boneName cannot be null.");
            ExposedList<Bone> bones = this.bones;
            for (int i = 0, n = bones.Count; i < n; i++)
            {
                Bone bone = bones.Items[i];
                if (bone.data.name == boneName) return bone;
            }
            return null;
        }

        /// <returns>-1 if the bone was not found.</returns>
        public int FindBoneIndex(String boneName)
        {
            if (boneName == null) throw new ArgumentNullException("boneName cannot be null.");
            ExposedList<Bone> bones = this.bones;
            for (int i = 0, n = bones.Count; i < n; i++)
                if (bones.Items[i].data.name == boneName) return i;
            return -1;
        }

        /// <returns>May be null.</returns>
        public Slot FindSlot(String slotName)
        {
            if (slotName == null) throw new ArgumentNullException("slotName cannot be null.");
            ExposedList<Slot> slots = this.slots;
            for (int i = 0, n = slots.Count; i < n; i++)
            {
                Slot slot = slots.Items[i];
                if (slot.data.name == slotName) return slot;
            }
            return null;
        }

        /// <returns>-1 if the bone was not found.</returns>
        public int FindSlotIndex(String slotName)
        {
            if (slotName == null) throw new ArgumentNullException("slotName cannot be null.");
            ExposedList<Slot> slots = this.slots;
            for (int i = 0, n = slots.Count; i < n; i++)
                if (slots.Items[i].data.name.Equals(slotName)) return i;
            return -1;
        }

        /// <summary>Sets a skin by name (see SetSkin).</summary>
        public void SetSkin(String skinName)
        {
            Skin skin = data.FindSkin(skinName);
            if (skin == null) throw new ArgumentException("Skin not found: " + skinName);
            SetSkin(skin);
        }

        /// <summary>Sets the skin used to look up attachments before looking in the {@link SkeletonData#getDefaultSkin() default 
        /// skin}. Attachmentsfrom the new skin are attached if the corresponding attachment from the old skin was attached. If 
        /// there was no old skin, each slot's setup mode attachment is attached from the new skin.</summary>
        /// <param name="newSkin">May be null.</param>
        public void SetSkin(Skin newSkin)
        {
            if (newSkin != null)
            {
                if (skin != null)
                    newSkin.AttachAll(this, skin);
                else
                {
                    ExposedList<Slot> slots = this.slots;
                    for (int i = 0, n = slots.Count; i < n; i++)
                    {
                        Slot slot = slots.Items[i];
                        String name = slot.data.attachmentName;
                        if (name != null)
                        {
                            Attachment attachment = newSkin.GetAttachment(i, name);
                            if (attachment != null) slot.Attachment = attachment;
                        }
                    }
                }
            }
            skin = newSkin;
        }

        /// <returns>May be null.</returns>
        public Attachment GetAttachment(String slotName, String attachmentName)
        {
            return GetAttachment(data.FindSlotIndex(slotName), attachmentName);
        }

        /// <returns>May be null.</returns>
        public Attachment GetAttachment(int slotIndex, String attachmentName)
        {
            if (attachmentName == null) throw new ArgumentNullException("attachmentName cannot be null.");
            if (skin != null)
            {
                Attachment attachment = skin.GetAttachment(slotIndex, attachmentName);
                if (attachment != null) return attachment;
            }
            if (data.defaultSkin != null) return data.defaultSkin.GetAttachment(slotIndex, attachmentName);
            return null;
        }

        /// <param name="attachmentName">May be null.</param>
        public void SetAttachment(String slotName, String attachmentName)
        {
            if (slotName == null) throw new ArgumentNullException("slotName cannot be null.");
            ExposedList<Slot> slots = this.slots;
            for (int i = 0, n = slots.Count; i < n; i++)
            {
                Slot slot = slots.Items[i];
                if (slot.data.name == slotName)
                {
                    Attachment attachment = null;
                    if (attachmentName != null)
                    {
                        attachment = GetAttachment(i, attachmentName);
                        if (attachment == null) throw new Exception("Attachment not found: " + attachmentName + ", for slot: " + slotName);
                    }
                    slot.Attachment = attachment;
                    return;
                }
            }
            throw new Exception("Slot not found: " + slotName);
        }

        /** @return May be null. */
        public IkConstraint FindIkConstraint(String constraintName)
        {
            if (constraintName == null) throw new ArgumentNullException("constraintName cannot be null.");
            ExposedList<IkConstraint> ikConstraints = this.ikConstraints;
            for (int i = 0, n = ikConstraints.Count; i < n; i++)
            {
                IkConstraint ikConstraint = ikConstraints.Items[i];
                if (ikConstraint.data.name == constraintName) return ikConstraint;
            }
            return null;
        }

        /** @return May be null. */
        public TransformConstraint FindTransformConstraint(String constraintName)
        {
            if (constraintName == null) throw new ArgumentNullException("constraintName cannot be null.");
            ExposedList<TransformConstraint> transformConstraints = this.transformConstraints;
            for (int i = 0, n = transformConstraints.Count; i < n; i++)
            {
                TransformConstraint transformConstraint = transformConstraints.Items[i];
                if (transformConstraint.data.name == constraintName) return transformConstraint;
            }
            return null;
        }

        public void Update(float delta)
        {
            time += delta;
        }
    }
}
