﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spine3_6_39;
using SpineViewerWPF;

public class Player_3_6_39 : IPlayer
{
    private Skeleton skeleton;
    private AnimationState state;
    private SkeletonRenderer skeletonRenderer;
    private Atlas atlas;
    private SkeletonData skeletonData;
    private AnimationStateData stateData;
    private SkeletonBinary binary;
    private SkeletonJson json;

    public void Initialize()
    {
        Player.Initialize(ref App.graphicsDevice, ref App.spriteBatch);
    }

    public void LoadContent(ContentManager contentManager)
    {
        skeletonRenderer = new SkeletonRenderer(App.graphicsDevice);
        skeletonRenderer.PremultipliedAlpha = App.globalValues.Alpha;

        atlas = new Atlas(App.globalValues.SelectAtlasFile, new XnaTextureLoader(App.graphicsDevice));

        if (Common.IsBinaryData(App.globalValues.SelectSpineFile))
        {
            binary = new SkeletonBinary(atlas);
            binary.Scale = App.globalValues.Scale;
            skeletonData = binary.ReadSkeletonData(App.globalValues.SelectSpineFile);
        }
        else
        {
            json = new SkeletonJson(atlas);
            json.Scale = App.globalValues.Scale;
            skeletonData = json.ReadSkeletonData(App.globalValues.SelectSpineFile);
        }
        App.globalValues.SpineVersion = skeletonData.Version;
        skeleton = new Skeleton(skeletonData);

        Common.SetInitLocation(skeleton.Data.Height);
        App.globalValues.FileHash = skeleton.Data.Hash;

        stateData = new AnimationStateData(skeleton.Data);

        state = new AnimationState(stateData);

        App.globalValues.AnimeList = state.Data.skeletonData.Animations.Select(x => x.Name).ToList();
        App.globalValues.SkinList = state.Data.skeletonData.Skins.Select(x => x.Name).ToList();

        if (App.globalValues.SelectAnimeName != "")
        {
            state.SetAnimation(0, App.globalValues.SelectAnimeName, App.globalValues.IsLoop);
        }
        else
        {
            state.SetAnimation(0, state.Data.skeletonData.animations.Items[0].name, App.globalValues.IsLoop);
        }

        if (App.isNew)
        {
            MainWindow.SetCBAnimeName();
        }
        App.isNew = false;

    }



    public void Update(GameTime gameTime)
    {
        if (App.globalValues.SelectAnimeName != "" && App.globalValues.SetAnime)
        {
            state.ClearTracks();
            skeleton.SetToSetupPose();
            state.SetAnimation(0, App.globalValues.SelectAnimeName, App.globalValues.IsLoop);
            App.globalValues.SetAnime = false;
        }

        if (App.globalValues.SelectSkin != "" && App.globalValues.SetSkin)
        {
            skeleton.SetSkin(App.globalValues.SelectSkin);
            skeleton.SetSlotsToSetupPose();
            App.globalValues.SetSkin = false;
        }
        if (App.globalValues.SelectSpineVersion != "3.6.39" || App.globalValues.FileHash != skeleton.Data.Hash)
        {
            state = null;
            skeletonRenderer = null;
            return;
        }
        App.graphicsDevice.Clear(Color.Transparent);

        Player.DrawBG(ref App.spriteBatch);
        App.globalValues.TimeScale = App.globalValues.Speed / 30f;

        state.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);

        state.Apply(skeleton);
        if (binary != null)
        {
            if (App.globalValues.Scale != binary.Scale)
            {
                binary.Scale = App.globalValues.Scale;
                skeletonData = binary.ReadSkeletonData(App.globalValues.SelectSpineFile);
                skeleton = new Skeleton(skeletonData);
            }
        }
        else if (json != null)
        {
            if (App.globalValues.Scale != json.Scale)
            {
                json.Scale = App.globalValues.Scale;
                skeletonData = json.ReadSkeletonData(App.globalValues.SelectSpineFile);
                skeleton = new Skeleton(skeletonData);
            }
        }

        skeleton.X = App.globalValues.PosX;
        skeleton.Y = App.globalValues.PosY;
        skeleton.FlipX = App.globalValues.FilpX;
        skeleton.FlipY = App.globalValues.FilpY;


        skeleton.RootBone.Rotation = App.globalValues.Rotation;
        skeleton.UpdateWorldTransform();
        skeletonRenderer.PremultipliedAlpha = App.globalValues.Alpha;
        if (skeletonRenderer.Effect is BasicEffect)
        {
            ((BasicEffect)skeletonRenderer.Effect).Projection = Matrix.CreateOrthographicOffCenter(0, App.graphicsDevice.Viewport.Width, App.graphicsDevice.Viewport.Height, 0, 1, 0);
        }
        else
        {
            skeletonRenderer.Effect.Parameters["Projection"].SetValue(Matrix.CreateOrthographicOffCenter(0, App.graphicsDevice.Viewport.Width, App.graphicsDevice.Viewport.Height, 0, 1, 0));
        }
        skeletonRenderer.Begin();
        skeletonRenderer.Draw(skeleton);
        skeletonRenderer.End();


    }

    public void Draw()
    {



        if (state != null)
        {
            TrackEntry entry = state.GetCurrent(0);
            float speed = App.globalValues.Speed / 30f;
            if (entry != null)
            {
                if (App.globalValues.IsRecoding && (App.globalValues.GifList != null || App.recordImageCount > 0) && !entry.IsComplete)
                {
                    if (App.recordImageCount == 1)
                    {
                        TrackEntry te = state.GetCurrent(0);
                        te.trackTime = 0;
                        App.globalValues.TimeScale = 1;
                        App.globalValues.Lock = 0;
                    }

                    Common.TakeRecodeScreenshot(App.graphicsDevice);
                }

                if (App.globalValues.IsRecoding && entry.IsComplete)
                {
                    state.TimeScale = 0;
                    App.globalValues.IsRecoding = false;
                    Common.RecodingEnd(entry.AnimationEnd);

                    state.TimeScale = speed;
                    App.globalValues.TimeScale = speed;
                }

                if (App.globalValues.TimeScale == 0)
                {
                    entry.TrackTime = entry.AnimationEnd * App.globalValues.Lock;
                    entry.TimeScale = 0;
                }
                else
                {
                    App.globalValues.Lock = entry.AnimationTime / entry.AnimationEnd;
                    entry.TimeScale = App.globalValues.TimeScale;
                }
                App.globalValues.LoadingProcess = $"{Math.Round(entry.AnimationTime / entry.AnimationEnd * 100, 2)}%";
            }
        }


    }

    public void ChangeSet()
    {
        App.appXC.ContentManager.Dispose();
        atlas.Dispose();
        atlas = null;
        App.appXC.LoadContent.Invoke(App.appXC.ContentManager);
    }

    public void SizeChange()
    {
        if (App.graphicsDevice != null)
            Player.UserControl_SizeChanged(ref App.graphicsDevice);
    }

    public void Dispose()
    {
        ChangeSet();
    }
}

