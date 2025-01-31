using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;


public class GlobalValue : INotifyPropertyChanged
{
    private string _SelectAtlasFile = "";
    private string _SelectSpineFile = "";
    private string _SpineVersion = "";
    private List<string> _AnimeList;
    private List<string> _SkinList;
    private float _Scale;
    private double _ViewScale;
    private int _Speed = 30;
    private float _PosX = 0;
    private float _PosY = 0;
    private float _PosBGX = 0;
    private float _PosBGY = 0;
    private bool _UseBG = false;
    private string _SelectBG = "";
    private bool _ControlBG = false;
    private bool _Alpha;
    private bool _IsLoop;
    private string _SelectAnimeName = "";
    private string _SelectSkin = "";
    private float _TimeScale = 1;
    private string _SelectSpineVersion = "";
    private double _FrameWidth;
    private double _FrameHeight;
    private bool _PreMultiplyAlpha;
    private bool _SetSkin = false;
    private bool _SetAnime = false;
    private string _FileHash = "";
    private string _ExportType = "Gif";
    private string _ExportPath = "";
    private string _LoadingProcess = "0%";
    private float _Lock = 0f;
    private bool _IsRecoding = false;
    private bool _FilpX = false;
    private bool _FilpY = false;
    private float _RedcodePanelWidth = 280f;
    private float _Rotation = 0;
    private bool _UseCache = false;

    private List<Texture2D> _GifList;


    public string SelectAtlasFile
    {
        get
        {
            return _SelectAtlasFile;
        }
        set
        {
            if (_SelectAtlasFile != value)
            {
                _SelectAtlasFile = value;
                OnPropertyChanged(nameof(SelectAtlasFile));
            }
        }
    }

    public string SelectSpineFile
    {
        get
        {
            return _SelectSpineFile;
        }
        set
        {
            if (_SelectSpineFile != value)
            {
                _SelectSpineFile = value;
                OnPropertyChanged(nameof(SelectSpineFile));
            }
        }
    }

    public string SpineVersion
    {
        get
        {
            return _SpineVersion;
        }
        set
        {
            if (_SpineVersion != value)
            {
                _SpineVersion = value;
                OnPropertyChanged(nameof(SpineVersion));
            }
        }
    }


    public List<string> AnimeList
    {
        get
        {
            return _AnimeList;
        }
        set
        {
            if (_AnimeList != value)
            {
                _AnimeList = value;
                OnPropertyChanged(nameof(AnimeList));
            }
        }
    }
    public List<string> SkinList
    {
        get
        {
            return _SkinList;
        }
        set
        {
            if (_SkinList != value)
            {
                _SkinList = value;
                OnPropertyChanged(nameof(SkinList));
            }
        }
    }
    public float Scale
    {
        get
        {
            return _Scale;
        }
        set
        {
            if (_Scale != value)
            {
                _Scale = (float)Math.Round(value, 2);
                OnPropertyChanged(nameof(Scale));
            }
        }
    }
    public double ViewScale
    {
        get
        {
            return _ViewScale;
        }
        set
        {
            if (_ViewScale != value)
            {
                _ViewScale = Math.Round(value, 2);
                OnPropertyChanged(nameof(ViewScale));
            }
        }
    }

    public int Speed
    {
        get
        {
            return _Speed;
        }
        set
        {
            if (int.TryParse(value.ToString(), out _Speed))
            {
                if (_Speed != value)
                {
                    _Speed = value;
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }
    }
    public float PosX
    {
        get
        {
            return _PosX;
        }
        set
        {
            if (_PosX != value)
            {
                _PosX = (float)Math.Round(value, 2);
                OnPropertyChanged(nameof(PosX));
            }
        }
    }
    public float PosY
    {
        get
        {
            return _PosY;
        }
        set
        {
            if (_PosY != value)
            {
                _PosY = (float)Math.Round(value, 2);
                OnPropertyChanged(nameof(PosY));
            }
        }
    }

    public float PosBGX
    {
        get
        {
            return _PosBGX;
        }
        set
        {
            if (_PosBGX != value)
            {
                _PosBGX = (float)Math.Round(value, 2);
                OnPropertyChanged(nameof(PosBGX));
            }
        }
    }
    public float PosBGY
    {
        get
        {
            return _PosBGY;
        }
        set
        {
            if (_PosBGY != value)
            {
                _PosBGY = (float)Math.Round(value, 2);
                OnPropertyChanged(nameof(PosBGY));
            }
        }
    }
    public bool Alpha
    {
        get
        {
            return _Alpha;
        }
        set
        {
            if (_Alpha != value)
            {
                _Alpha = value;
                OnPropertyChanged(nameof(Alpha));
            }
        }
    }
    public bool UseBG
    {
        get
        {
            return _UseBG;
        }
        set
        {
            if (_UseBG != value)
            {
                _UseBG = value;
                OnPropertyChanged(nameof(UseBG));
            }
        }
    }
    public bool ControlBG
    {
        get
        {
            return _ControlBG;
        }
        set
        {
            if (_ControlBG != value)
            {
                _ControlBG = value;
                OnPropertyChanged(nameof(ControlBG));
            }
        }
    }
    public bool IsLoop
    {
        get
        {
            return _IsLoop;
        }
        set
        {
            if (_IsLoop != value)
            {
                _IsLoop = value;
                OnPropertyChanged(nameof(IsLoop));
            }
        }
    }
    public string SelectAnimeName
    {
        get
        {
            return _SelectAnimeName;
        }
        set
        {
            if (_SelectAnimeName != value)
            {
                _SelectAnimeName = value;
                OnPropertyChanged(nameof(SelectAnimeName));
            }
        }
    }
    public string SelectSkin
    {
        get
        {
            return _SelectSkin;
        }
        set
        {
            if (_SelectSkin != value)
            {
                _SelectSkin = value;
                OnPropertyChanged(nameof(SelectSkin));
            }
        }
    }
    public string SelectBG
    {
        get
        {
            return _SelectBG;
        }
        set
        {
            if (_SelectBG != value)
            {
                _SelectBG = value;
                OnPropertyChanged(nameof(SelectBG));
            }
        }
    }

    public float TimeScale
    {
        get
        {
            return _TimeScale;
        }
        set
        {
            if (_TimeScale != value)
            {
                _TimeScale = value;
                OnPropertyChanged(nameof(TimeScale));
            }
        }
    }
    public string SelectSpineVersion
    {
        get
        {
            return _SelectSpineVersion;
        }
        set
        {
            if (_SelectSpineVersion != value)
            {
                _SelectSpineVersion = value;
                OnPropertyChanged(nameof(SelectSpineVersion));
            }
        }
    }
    public double FrameWidth
    {
        get
        {
            return _FrameWidth;
        }
        set
        {
            if (_FrameWidth != value)
            {
                _FrameWidth = value;
                OnPropertyChanged(nameof(FrameWidth));
            }
        }
    }
    public double FrameHeight
    {
        get
        {
            return _FrameHeight;
        }
        set
        {
            if (_FrameHeight != value)
            {
                _FrameHeight = value;
                OnPropertyChanged(nameof(FrameHeight));
            }
        }
    }

    public bool PreMultiplyAlpha
    {
        get
        {
            return _PreMultiplyAlpha;
        }
        set
        {
            if (_PreMultiplyAlpha != value)
            {
                _PreMultiplyAlpha = value;
                OnPropertyChanged(nameof(PreMultiplyAlpha));
            }
        }
    }

    public bool SetSkin
    {
        get
        {
            return _SetSkin;
        }
        set
        {
            if (_SetSkin != value)
            {
                _SetSkin = value;
                OnPropertyChanged(nameof(SetSkin));
            }
        }
    }

    public bool SetAnime
    {
        get
        {
            return _SetAnime;
        }
        set
        {
            if (_SetAnime != value)
            {
                _SetAnime = value;
                OnPropertyChanged(nameof(SetAnime));
            }
        }
    }

    public string FileHash
    {
        get
        {
            return _FileHash;
        }
        set
        {
            if (_FileHash != value)
            {
                _FileHash = value;
                OnPropertyChanged(nameof(FileHash));
            }
        }
    }

    public string ExportType
    {
        get
        {
            return _ExportType;
        }
        set
        {
            if (_ExportType != value)
            {
                _ExportType = value;
                OnPropertyChanged(nameof(ExportType));
            }
        }
    }

    public string ExportPath
    {
        get
        {
            return _ExportPath;
        }
        set
        {
            if (_ExportPath != value)
            {
                _ExportPath = value;
                OnPropertyChanged(nameof(ExportPath));
            }
        }
    }
    public string LoadingProcess
    {
        get
        {
            return _LoadingProcess;
        }
        set
        {
            if (_LoadingProcess != value)
            {
                _LoadingProcess = value;
                OnPropertyChanged(nameof(LoadingProcess));
            }

        }
    }

    public float Lock
    {
        get
        {
            return _Lock;
        }
        set
        {
            if (float.TryParse(value.ToString(), out _Lock))
            {
                _Lock = (float)Math.Round(value, 2);
                OnPropertyChanged(nameof(Lock));
            }
        }
    }

    public List<Texture2D> GifList
    {

        get
        {
            _GifList ??= [];


            return _GifList;
        }
        set
        {
            if (_GifList != value)
            {
                _GifList = value;
            }


        }
    }

    public bool IsRecoding
    {
        get
        {
            return _IsRecoding;
        }
        set
        {
            if (_IsRecoding != value)
            {
                _IsRecoding = value;
                OnPropertyChanged(nameof(IsRecoding));
            }
        }
    }

    public bool FilpX
    {
        get
        {
            return _FilpX;
        }
        set
        {
            if (_FilpX != value)
            {
                _FilpX = value;
                OnPropertyChanged(nameof(FilpX));
            }
        }
    }

    public bool FilpY
    {
        get
        {
            return _FilpY;
        }
        set
        {
            if (_FilpY != value)
            {
                _FilpY = value;
                OnPropertyChanged(nameof(FilpY));
            }
        }
    }

    public float RedcodePanelWidth
    {
        get
        {
            return _RedcodePanelWidth;
        }
        set
        {
            if (float.TryParse(value.ToString(), out _RedcodePanelWidth))
            {
                if (_RedcodePanelWidth != value)
                {
                    _RedcodePanelWidth = value;
                    OnPropertyChanged(nameof(RedcodePanelWidth));
                }
            }
        }
    }

    public float Rotation
    {
        get
        {
            return _Rotation;
        }
        set
        {
            if (float.TryParse(value.ToString(), out _Rotation))
            {
                if (_Rotation != value)
                {
                    _Rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }
    }

    public bool UseCache
    {
        get
        {
            return _UseCache;
        }
        set
        {
            if (_UseCache != value)
            {
                _UseCache = value;
                OnPropertyChanged(nameof(UseCache));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


}

