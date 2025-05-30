/******************************************************************************
 * Spine Runtimes Software License v2.5
 *
 * Copyright (c) 2013-2016, Esoteric Software
 * All rights reserved.
 *
 * You are granted a perpetual, non-exclusive, non-sublicensable, and
 * non-transferable license to use, install, execute, and perform the Spine
 * Runtimes software and derivative works solely for personal or internal
 * use. Without the written permission of Esoteric Software (see Section 2 of
 * the Spine Software License Agreement), you may not (a) modify, translate,
 * adapt, or develop new applications using the Spine Runtimes or otherwise
 * create derivative works or improvements of the Spine Runtimes or (b) remove,
 * delete, alter, or obscure any trademarks or any copyright, trademark, patent,
 * or other intellectual property or proprietary rights notices on or in the
 * Software, including any copy thereof. Redistributions in binary or source
 * form must include this license and terms.
 *
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES, BUSINESS INTERRUPTION, OR LOSS OF
 * USE, DATA, OR PROFITS) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
 * IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;

namespace Spine3_6_39
{

    /// <summary>
    /// An AttachmentLoader that configures attachments using texture regions from an Atlas.
    /// See <a href='http://esotericsoftware.com/spine-loading-skeleton-data#JSON-and-binary-data'>Loading Skeleton Data</a> in the Spine Runtimes Guide.
    /// </summary>
    public class AtlasAttachmentLoader : AttachmentLoader
    {
        private Atlas[] atlasArray;

        public AtlasAttachmentLoader(params Atlas[] atlasArray)
        {
            if (atlasArray == null) throw new ArgumentNullException("atlas array cannot be null.");
            this.atlasArray = atlasArray;
        }

        public RegionAttachment NewRegionAttachment(Skin skin, string name, string path)
        {
            AtlasRegion region = FindRegion(path);
            if (region == null) throw new ArgumentException(string.Format("Region not found in atlas: {0} (region attachment: {1})", path, name));
            RegionAttachment attachment = new RegionAttachment(name);
            attachment.RendererObject = region;
            attachment.SetUVs(region.u, region.v, region.u2, region.v2, region.rotate);
            attachment.regionOffsetX = region.offsetX;
            attachment.regionOffsetY = region.offsetY;
            attachment.regionWidth = region.width;
            attachment.regionHeight = region.height;
            attachment.regionOriginalWidth = region.originalWidth;
            attachment.regionOriginalHeight = region.originalHeight;
            return attachment;
        }

        public MeshAttachment NewMeshAttachment(Skin skin, string name, string path)
        {
            AtlasRegion region = FindRegion(path);
            if (region == null) throw new ArgumentException(string.Format("Region not found in atlas: {0} (region attachment: {1})", path, name));
            MeshAttachment attachment = new MeshAttachment(name);
            attachment.RendererObject = region;
            attachment.RegionU = region.u;
            attachment.RegionV = region.v;
            attachment.RegionU2 = region.u2;
            attachment.RegionV2 = region.v2;
            attachment.RegionRotate = region.rotate;
            attachment.regionOffsetX = region.offsetX;
            attachment.regionOffsetY = region.offsetY;
            attachment.regionWidth = region.width;
            attachment.regionHeight = region.height;
            attachment.regionOriginalWidth = region.originalWidth;
            attachment.regionOriginalHeight = region.originalHeight;
            return attachment;
        }

        public BoundingBoxAttachment NewBoundingBoxAttachment(Skin skin, string name)
        {
            return new BoundingBoxAttachment(name);
        }

        public PathAttachment NewPathAttachment(Skin skin, string name)
        {
            return new PathAttachment(name);
        }

        public PointAttachment NewPointAttachment(Skin skin, string name)
        {
            return new PointAttachment(name);
        }

        public ClippingAttachment NewClippingAttachment(Skin skin, string name)
        {
            return new ClippingAttachment(name);
        }

        public AtlasRegion FindRegion(string name)
        {
            AtlasRegion region;

            for (int i = 0; i < atlasArray.Length; i++)
            {
                region = atlasArray[i].FindRegion(name);
                if (region != null)
                    return region;
            }

            return null;
        }
    }
}
