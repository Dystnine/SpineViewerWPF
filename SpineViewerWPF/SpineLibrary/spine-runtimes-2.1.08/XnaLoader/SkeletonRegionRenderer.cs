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

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spine2_1_08
{
    /// <summary>Draws region attachments.</summary>
    public class SkeletonRegionRenderer
    {
        GraphicsDevice device;
        RegionBatcher batcher;
        RasterizerState rasterizerState;
        float[] vertices = new float[8];
        BlendState defaultBlendState;

        BasicEffect effect;
        public BasicEffect Effect { get { return effect; } set { effect = value; } }

        private bool premultipliedAlpha;
        public bool PremultipliedAlpha { get { return premultipliedAlpha; } set { premultipliedAlpha = value; } }

        public SkeletonRegionRenderer(GraphicsDevice device)
        {
            this.device = device;

            batcher = new RegionBatcher();

            effect = new BasicEffect(device);
            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;

            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;

            Bone.yDown = true;
        }

        public void Begin()
        {
            defaultBlendState = premultipliedAlpha ? BlendState.AlphaBlend : BlendState.NonPremultiplied;

            device.RasterizerState = rasterizerState;
            device.BlendState = defaultBlendState;

            effect.Projection = Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, device.Viewport.Height, 0, 1, 0);
        }

        public void End()
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                batcher.Draw(device);
            }
        }

        public void Draw(Skeleton skeleton)
        {
            List<Slot> drawOrder = skeleton.DrawOrder;
            float skeletonR = skeleton.R, skeletonG = skeleton.G, skeletonB = skeleton.B, skeletonA = skeleton.A;
            for (int i = 0, n = drawOrder.Count; i < n; i++)
            {
                Slot slot = drawOrder[i];
                RegionAttachment regionAttachment = slot.Attachment as RegionAttachment;
                if (regionAttachment != null)
                {
                    BlendState blend = slot.Data.AdditiveBlending ? BlendState.Additive : defaultBlendState;
                    if (device.BlendState != blend)
                    {
                        End();
                        device.BlendState = blend;
                    }

                    RegionItem item = batcher.NextItem();

                    AtlasRegion region = (AtlasRegion)regionAttachment.RendererObject;
                    item.texture = (Texture2D)region.page.rendererObject;

                    Color color;
                    float a = skeletonA * slot.A;
                    if (premultipliedAlpha)
                        color = new Color(skeletonR * slot.R * a, skeletonG * slot.G * a, skeletonB * slot.B * a, a);
                    else
                        color = new Color(skeletonR * slot.R, skeletonG * slot.G, skeletonB * slot.B, a);
                    item.vertexTL.Color = color;
                    item.vertexBL.Color = color;
                    item.vertexBR.Color = color;
                    item.vertexTR.Color = color;

                    float[] vertices = this.vertices;
                    regionAttachment.ComputeWorldVertices(slot.Bone, vertices);
                    item.vertexTL.Position.X = vertices[RegionAttachment.X1];
                    item.vertexTL.Position.Y = vertices[RegionAttachment.Y1];
                    item.vertexTL.Position.Z = 0;
                    item.vertexBL.Position.X = vertices[RegionAttachment.X2];
                    item.vertexBL.Position.Y = vertices[RegionAttachment.Y2];
                    item.vertexBL.Position.Z = 0;
                    item.vertexBR.Position.X = vertices[RegionAttachment.X3];
                    item.vertexBR.Position.Y = vertices[RegionAttachment.Y3];
                    item.vertexBR.Position.Z = 0;
                    item.vertexTR.Position.X = vertices[RegionAttachment.X4];
                    item.vertexTR.Position.Y = vertices[RegionAttachment.Y4];
                    item.vertexTR.Position.Z = 0;

                    float[] uvs = regionAttachment.UVs;
                    item.vertexTL.TextureCoordinate.X = uvs[RegionAttachment.X1];
                    item.vertexTL.TextureCoordinate.Y = uvs[RegionAttachment.Y1];
                    item.vertexBL.TextureCoordinate.X = uvs[RegionAttachment.X2];
                    item.vertexBL.TextureCoordinate.Y = uvs[RegionAttachment.Y2];
                    item.vertexBR.TextureCoordinate.X = uvs[RegionAttachment.X3];
                    item.vertexBR.TextureCoordinate.Y = uvs[RegionAttachment.Y3];
                    item.vertexTR.TextureCoordinate.X = uvs[RegionAttachment.X4];
                    item.vertexTR.TextureCoordinate.Y = uvs[RegionAttachment.Y4];
                }
            }
        }
    }
}
