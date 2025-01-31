﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Spine3_2_xx;
using SpineViewerWPF;

public class Player_3_2_xx : IPlayer
{
    private Skeleton skeleton;
    private AnimationState state;
    private SkeletonMeshRenderer skeletonRenderer;
    private ExposedList<Animation> listAnimation;
    private ExposedList<Skin> listSkin;
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
        skeletonRenderer = new SkeletonMeshRenderer(App.graphicsDevice);
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

        List<string> AnimationNames = new List<string>();
        listAnimation = state.Data.skeletonData.Animations;
        foreach (Animation An in listAnimation)
        {
            AnimationNames.Add(An.name);
        }
        App.globalValues.AnimeList = AnimationNames;

        List<string> SkinNames = new List<string>();
        listSkin = state.Data.skeletonData.Skins;
        foreach (Skin Sk in listSkin)
        {
            SkinNames.Add(Sk.name);
        }
        App.globalValues.SkinList = SkinNames;

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
        if (App.globalValues.SelectSpineVersion != "3.2.xx" || App.globalValues.FileHash != skeleton.Data.Hash)
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

                if (App.globalValues.IsRecoding && (App.globalValues.GifList != null || App.recordImageCount > 0) && entry.LastTime < entry.EndTime)
                {
                    if (App.recordImageCount == 1)
                    {
                        TrackEntry te = state.GetCurrent(0);
                        te.Time = 0;
                        App.globalValues.TimeScale = speed;
                        App.globalValues.Lock = 0;
                    }

                    Common.TakeRecodeScreenshot(App.graphicsDevice);
                }

                if (App.globalValues.IsRecoding && entry.LastTime >= entry.EndTime)
                {
                    state.TimeScale = 0;
                    App.globalValues.IsRecoding = false;
                    Common.RecodingEnd(entry.EndTime);

                    state.TimeScale = speed;
                    App.globalValues.TimeScale = speed;
                }

                if (App.globalValues.TimeScale == 0)
                {
                    entry.Time = entry.EndTime * App.globalValues.Lock;
                    entry.TimeScale = 0;
                }
                else
                {
                    App.globalValues.Lock = (entry.LastTime % entry.EndTime) / entry.EndTime;
                    entry.TimeScale = App.globalValues.TimeScale;
                }
                App.globalValues.LoadingProcess = $"{Math.Round((entry.Time % entry.EndTime) / entry.EndTime * 100, 2)}%";
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

