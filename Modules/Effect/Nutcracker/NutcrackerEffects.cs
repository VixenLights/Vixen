using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using VixenModules.Effect.Nutcracker;

namespace VixenModules.Effect.Nutcracker
{    
    public class NutcrackerEffects
    {
        private NutcrackerData _data = null;
        private long _state;
        private int _lastPeriod;
        private int _renderPeriod;
        private List<List<Color>> _pixels = new List<List<Color>>();
        private List<List<Color>> _tempbuf = new List<List<Color>>();
        private double pi2 = Math.PI * 2;
        private int[] FireBuffer, WaveBuffer0, WaveBuffer1, WaveBuffer2 = new int[1];
        private Random random = new Random();
        private List<Color> FirePalette = new List<Color>();

        public enum Effects
        {
            Bars,
            Butterfly,
            ColorWash,
            Fire,
            Garlands,
            Life,
            Meteors,
            Fireworks
        }
        
        public enum PreviewType
        {
            None,
            Tree90,
            Tree180,
            Tree270,
            Tree360,
            Arch
        }

        public NutcrackerEffects()
        {
            _state = 0;
            _lastPeriod = 0;
            _renderPeriod = 0;
        }

        public NutcrackerEffects(NutcrackerData data)
        {
            Data = data;
        }

        public NutcrackerData Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #region Properties

        public long State
        {
            get { return _state; }
            set { _state = value; }
        }

        public List<List<Color>> Pixels
        {
            get { return _pixels; }
            set { _pixels = value; }
        }

        public int RenderPeriod
        {
            get { return _renderPeriod; }
            set { _renderPeriod = value; }
        }

        public Palette Palette
        {
            get { return Data.Palette; }
            set { Data.Palette = value; }
        }

        #endregion

        #region Nutcracker Utilities

        private int rand()
        {
            return random.Next();
        }

        // return a random number between 0 and 1 inclusive
        private double rand01()
        {
            return random.NextDouble();
        }

        public void SetState(int period, int NewSpeed, bool ResetState)
        {
            if (ResetState)
            {
                State = 0;
            }
            else
            {
                State += (period - _lastPeriod) * NewSpeed;
            }
            Data.Speed = NewSpeed;

            _lastPeriod = period;
        }

        public void SetNextState(bool ResetState)
        {
            if (Data != null)
            {
                RenderPeriod += 1;

                SetState(RenderPeriod, (int)Data.Speed, ResetState);
            }
        }

        public int BufferHt
        {
            get 
            {
                if (_pixels.Count() > 0)
                    return _pixels[0].Count();
                else
                    return 0;
            }
        }

        public int BufferWi
        {
            get { return _pixels.Count(); }
        }

        // return a value between c1 and c2
        int ChannelBlend(int c1, int c2, double ratio)
        {
            return c1 + (int)Math.Floor(ratio * (double)(c2 - c1) + 0.5);
        }

        public int GetColorCount()
        {
            return Palette.Count();
        }

        public Color Get2ColorBlend(int coloridx1, int coloridx2, double ratio)
        {
            Color c1,c2;
            c1 = Palette.GetColor(coloridx1);
            c2 = Palette.GetColor(coloridx2);

            return Color.FromArgb(ChannelBlend(c1.R,c2.R,ratio), ChannelBlend(c1.G,c2.G,ratio), ChannelBlend(c1.B,c2.B,ratio));;
        }

        // 0 <= n < 1
        public Color GetMultiColorBlend(double n, bool circular)
        {
            int colorcnt = GetColorCount();
            if (colorcnt <= 1)
            {
                return Palette.GetColor(0);
            }
            if (n >= 1.0) n = 0.99999;
            if (n < 0.0) n = 0.0;
            double realidx = circular ? n*colorcnt : n*(colorcnt-1);
            int coloridx1 = (int)Math.Floor(realidx);
            int coloridx2 = (coloridx1+1) % colorcnt;
            double ratio = realidx - (double)coloridx1;
            return Get2ColorBlend(coloridx1, coloridx2, ratio);
        }

        #endregion

        #region Nutcracker Effects

        public void RenderBars(int PaletteRepeat, int Direction, bool Highlight, bool Show3D)
        {
            int x, y, n, pixel_ratio, ColorIdx;
            bool IsMovingDown, IsHighlightRow;
            HSV hsv;
            int colorcnt = Palette.Count();
            // If we don't have any colors, we can't do anything!
            //if (colorcnt == 0) return;
            int BarCount = PaletteRepeat * colorcnt;
            int BarHt = BufferHt / BarCount + 1;
            int HalfHt = BufferHt/2;
            int BlockHt = colorcnt * BarHt;
            int f_offset = (int)(_state/4 % BlockHt);
            for (y = 0; y < BufferHt; y++)
            {
                switch (Direction)
                {
                case 1:
                    IsMovingDown=true;
                    break;
                case 2:
                    IsMovingDown=(y <= HalfHt);
                    break;
                case 3:
                    IsMovingDown=(y > HalfHt);
                    break;
                default:
                    IsMovingDown=false;
                    break;
                }
                if (IsMovingDown)
                {
                    n = y + f_offset;
                    pixel_ratio = BarHt - n % BarHt - 1;
                    IsHighlightRow = n % BarHt == 0;
                }
                else
                {
                    n = y - f_offset + BlockHt;
                    pixel_ratio = n % BarHt;
                    IsHighlightRow = (n % BarHt == BarHt-1); // || (y == BufferHt-1);
                }
                ColorIdx = (n % BlockHt) / BarHt;
                hsv = Palette.GetHSV(ColorIdx);
                if (Highlight && IsHighlightRow) hsv.Saturation = 0f;
                if (Show3D) hsv.Value *= (float)pixel_ratio / (float)BarHt;
                for (x = 0; x < BufferWi; x++)
                {
                    SetPixel(x, y, hsv);
                }
            }
        }

        public void RenderGarlands(int GarlandType, int Spacing)
        {
            int x, y, yadj, ylimit, ring;
            double ratio;
            Color color;
            int PixelSpacing = Spacing * BufferHt / 100 + 3;
            int limit = BufferHt * PixelSpacing * 4;
            int GarlandsState = (limit - ((int)_state % limit)) / 4;
            for (ring = 0; ring < BufferHt; ring++)
            {
                ratio = (double)ring/(double)BufferHt;
                color = GetMultiColorBlend(ratio, false);
                y = GarlandsState - ring * PixelSpacing;
                ylimit = BufferHt - ring - 1;
                for (x = 0; x < BufferWi; x++)
                {
                    yadj = y;
                    switch (GarlandType)
                    {
                    case 1:
                        switch (x%5)
                        {
                        case 2:
                            yadj-=2;
                            break;
                        case 1:
                        case 3:
                            yadj-=1;
                            break;
                        }
                        break;
                    case 2:
                        switch (x%5)
                        {
                        case 2:
                            yadj-=4;
                            break;
                        case 1:
                        case 3:
                            yadj-=2;
                            break;
                        }
                        break;
                    case 3:
                        switch (x%6)
                        {
                        case 3:
                            yadj-=6;
                            break;
                        case 2:
                        case 4:
                            yadj-=4;
                            break;
                        case 1:
                        case 5:
                            yadj-=2;
                            break;
                        }
                        break;
                    case 4:
                        switch (x%5)
                        {
                        case 1:
                        case 3:
                            yadj-=2;
                            break;
                        }
                        break;
                    }
                    if (yadj < ylimit) yadj = ylimit;
                    if (yadj < BufferHt) SetPixel(x, yadj, color);
                }
            }
        }

        public void RenderButterfly(int ColorScheme, int Style, int Chunks, int Skip)
        {
            int x, y, d;
            double n, x1, y1, f;
            double h = 0.0;
            Color color;
            HSV hsv = new HSV();
            int maxframe = BufferHt * 2;
            int frame = (int)(((double)BufferHt * (double)State / 200.0) % (double)maxframe);
            double offset = (double)State / 100.0;
            for (x = 0; x < BufferWi; x++)
            {
                for (y = 0; y < BufferHt; y++)
                {
                    switch (Style)
                    {
                        case 1:
                            n = Math.Abs((x * x - y * y) * Math.Sin(offset + ((x + y) * pi2 / (BufferHt + BufferWi))));
                            d = x * x + y * y + 1;
                            h = n / d;
                            break;
                        case 2:
                            f = (frame < maxframe / 2) ? frame + 1 : maxframe - frame;
                            x1 = ((double)x - BufferWi / 2.0) / f;
                            y1 = ((double)y - BufferHt / 2.0) / f;
                            h = Math.Sqrt(x1 * x1 + y1 * y1);
                            break;
                        case 3:
                            f = (frame < maxframe / 2) ? frame + 1 : maxframe - frame;
                            f = f * 0.1 + (double)BufferHt / 60.0;
                            x1 = (x - BufferWi / 2.0) / f;
                            y1 = (y - BufferHt / 2.0) / f;
                            h = Math.Sin(x1) * Math.Cos(y1);
                            break;

                    }
                    hsv.Saturation = 1.0f;
                    hsv.Value = 1.0f;
                    if (Chunks <= 1 || ((int)(h * Chunks)) % Skip != 0)
                    {
                        if (ColorScheme == 0)
                        {
                            hsv.Hue = (float)h;
                            SetPixel(x, y, hsv);
                        }
                        else
                        {
                            color = GetMultiColorBlend(h, false);
                            SetPixel(x, y, color);
                        }
                    }
                }
            }
        }

        public void RenderColorWash(bool HorizFade, bool VertFade, int RepeatCount)
        {
            int SpeedFactor = 200;
            int x, y;
            Color color;
            HSV hsv2 = new HSV();
            int colorcnt = GetColorCount();
            int CycleLen = colorcnt * SpeedFactor;
            if (State > (colorcnt - 1) * SpeedFactor * RepeatCount && RepeatCount < 10)
            {
                color = GetMultiColorBlend((double)(RepeatCount % 2), false);
            }
            else
            {
                color = GetMultiColorBlend((double)(State % CycleLen) / (double)CycleLen, true);
            }
            HSV hsv = HSV.ColorToHSV(color);
            double HalfHt = (double)(BufferHt - 1) / 2.0;
            double HalfWi = (double)(BufferWi - 1) / 2.0;
            for (x = 0; x < BufferWi; x++)
            {
                for (y = 0; y < BufferHt; y++)
                {
                    hsv2.SetToHSV(hsv);
                    if (HorizFade) hsv2.Value *= (float)(1.0 - Math.Abs(HalfWi - x) / HalfWi);
                    if (VertFade) hsv2.Value *= (float)(1.0 - Math.Abs(HalfHt - y) / HalfHt);
                    SetPixel(x, y, hsv2);
                }
            }
        }

        #region Fire

        // 0 <= x < BufferWi
        // 0 <= y < BufferHt
        private void SetFireBuffer(int x, int y, int PaletteIdx){
            if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
            {
                FireBuffer[y*BufferWi+x] = PaletteIdx;
            }
        }
        
        // 0 <= x < BufferWi
        // 0 <= y < BufferHt
        private int GetFireBuffer(int x, int y)
        {
            if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
            {
                return FireBuffer[y*BufferWi+x];
            }
            return -1;
        }

        private void SetWaveBuffer1(int x, int y, int value)
        {
            if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
            {
                WaveBuffer1[y*BufferWi+x] = value;
            }
        }
        private void SetWaveBuffer2(int x, int y, int value)
        {
            if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
            {
                WaveBuffer2[y*BufferWi+x] = value;
            }
        }

        private int GetWaveBuffer1(int x, int y)
        {
            if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
            {
                return WaveBuffer1[y*BufferWi+x];
            }
            return -1;
        }

        private int GetWaveBuffer2(int x, int y)
        {
            if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
            {
                return WaveBuffer2[y*BufferWi+x];
            }
            return -1;
        }

        // 10 <= HeightPct <= 100
        private void RenderFire(int HeightPct)
        {
            int x,y,i,r,v1,v2,v3,v4,n,new_index;
            if (State == 0)
            {
                for (i=0; i < FireBuffer.Count(); i++)
                {
                    FireBuffer[i]=0;
                }
            }
            // build fire
            for (x=0; x<BufferWi; x++)
            {
                r=x%2==0 ? 190+(rand() % 10) : 100+(rand() % 50);
                SetFireBuffer(x,0,r);
            }
            int step=255*100/BufferHt/HeightPct;
            int sum;
            for (y=1; y<BufferHt; y++)
            {
                for (x=0; x<BufferWi; x++)
                {
                    v1=GetFireBuffer(x-1,y-1);
                    v2=GetFireBuffer(x+1,y-1);
                    v3=GetFireBuffer(x,y-1);
                    v4=GetFireBuffer(x,y-1);
                    n=0;
                    sum=0;
                    if(v1>=0)
                    {
                        sum+=v1;
                        n++;
                    }
                    if(v2>=0)
                    {
                        sum+=v2;
                        n++;
                    }
                    if(v3>=0)
                    {
                        sum+=v3;
                        n++;
                    }
                    if(v4>=0)
                    {
                        sum+=v4;
                        n++;
                    }
                    new_index=n > 0 ? sum / n : 0;
                    if (new_index > 0)
                    {
                        new_index+=(rand() % 100 < 20) ? step : -step;
                        if (new_index < 0) new_index=0;
                        if (new_index >= FirePalette.Count()) new_index = FirePalette.Count()-1;
                    }
                    SetFireBuffer(x,y,new_index);
                }
            }
            for (y=0; y<BufferHt; y++)
            {
                for (x=0; x<BufferWi; x++)
                {
                    //SetPixel(x,y,FirePalette[y]);
                    SetPixel(x,y,FirePalette[GetFireBuffer(x,y)]);
                }
            }
        }

        #endregion // Fire

        //
        // LIFE IS BROKEN
        //
        #region Life

        private int LastLifeCount, LastLifeType = 0;
        private long LastLifeState = 0;

        private int Life_CountNeighbors(int x0, int y0)
        {
            //     2   3   4
            //     1   X   5
            //     0   7   6
            int[] n_x = { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] n_y = { -1, 0, 1, 1, 1, 0, -1, -1 };
            int x, y, cnt = 0;
            for (int i = 0; i < 8; i++)
            {
                x = (x0 + n_x[i]) % BufferWi;
                y = (y0 + n_y[i]) % BufferHt;
                if (x < 0) x += BufferWi;
                if (y < 0) y += BufferHt;
                //if (GetTempPixelRGB(x, y) != 0) cnt++;
                if (GetTempPixel(x, y) != Color.Black) cnt++;
            }
            return cnt;
        }

        // use tempbuf for calculations
        public void RenderLife(int Count, int Type)
        {
            int i,x,y,cnt;
            bool isLive;
            Color color;
            Count = BufferWi * BufferHt * Count / 200 + 1;
            if (State == 0 || Count != LastLifeCount || Type != LastLifeType)
            {
                Console.WriteLine("RenderLife Init");
                // seed tempbuf
                LastLifeCount=Count;
                LastLifeType=Type;
                ClearTempBuf();
                for(i=0; i<Count; i++)
                {
                    x=rand() % BufferWi;
                    y=rand() % BufferHt;
                    color = GetMultiColorBlend(rand01(),false);
                    SetTempPixel(x,y,color);
                }
            }
            long TempState = State % 400 / 20;
            if (TempState == LastLifeState)
            {
                //Pixels=tempbuf;
                CopyTempBufToPixels();
                return;
            }
            else
            {
                LastLifeState = TempState;
            }

            for (x = 0; x < BufferWi; x++)
            {
                for (y=0; y < BufferHt; y++)
                {
                    color = GetTempPixel(x, y);
                    //isLive=(color.GetRGB() != 0);
                    isLive = (color != Color.Black);
                    cnt=Life_CountNeighbors(x,y);
                    switch (Type)
                    {
                    case 0:
                        // B3/S23
                        /*
                        Any live cell with fewer than two live neighbours dies, as if caused by under-population.
                        Any live cell with two or three live neighbours lives on to the next generation.
                        Any live cell with more than three live neighbours dies, as if by overcrowding.
                        Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                        */
                        if (isLive && cnt >= 2 && cnt <= 3)
                        {
                            SetPixel(x,y,color);
                        }
                        else if (!isLive && cnt == 3)
                        {
                            color = GetMultiColorBlend(rand01(), false);
                            SetPixel(x,y,color);
                        }
                        break;
                    case 1:
                        // B35/S236
                        if (isLive && (cnt == 2 || cnt == 3 || cnt == 6))
                        {
                            SetPixel(x,y,color);
                        }
                        else if (!isLive && (cnt == 3 || cnt == 5))
                        {
                            color = GetMultiColorBlend(rand01(), false);
                            SetPixel(x,y,color);
                        }
                        break;
                    case 2:
                        // B357/S1358
                        if (isLive && (cnt == 1 || cnt == 3 || cnt == 5 || cnt == 8))
                        {
                            SetPixel(x,y,color);
                        }
                        else if (!isLive && (cnt == 3 || cnt == 5 || cnt == 7))
                        {
                            color = GetMultiColorBlend(rand01(), false);
                            SetPixel(x,y,color);
                        }
                        break;
                    case 3:
                        // B378/S235678
                        if (isLive && (cnt == 2 || cnt == 3 || cnt >= 5))
                        {
                            SetPixel(x,y,color);
                        }
                        else if (!isLive && (cnt == 3 || cnt == 7 || cnt == 8))
                        {
                            color = GetMultiColorBlend(rand01(), false);
                            SetPixel(x,y,color);
                        }
                        break;
                    case 4:
                        // B25678/S5678
                        if (isLive && (cnt >= 5))
                        {
                            SetPixel(x,y,color);
                        }
                        else if (!isLive && (cnt == 2 || cnt >= 5))
                        {
                            color = GetMultiColorBlend(rand01(), false);
                            SetPixel(x,y,color);
                        }
                        break;
                    }
                }
            }
            // copy new life state to tempbuf
            CopyPixelsToTempBuf();
        }

        #endregion // Life

        #region Meteors

        List<MeteorClass> meteors = new List<MeteorClass>();

        // for meteor effect
        public class MeteorClass
        {

	        public int x;
	        public int y;
	        public HSV hsv = new HSV();

            public bool HasExpired(int tailLength) 
            {
                return (y + tailLength < 0);
            }
        }

        private void RenderMeteors(int MeteorType, int Count, int Length)
        {
            if (State == 0) 
                meteors.Clear();
            int mspeed = (int)State / 4;
            State -= mspeed * 4 - 1;

            // create new meteors
            HSV hsv = new HSV();
            HSV hsv0 = new HSV();
            HSV hsv1 = new HSV();
            hsv0 = Palette.GetHSV(0);
            hsv1 = Palette.GetHSV(1);
            int colorcnt = GetColorCount();
            Count = BufferWi * Count / 100;
            int TailLength = (BufferHt < 10) ? Length / 10 : BufferHt * Length / 100;
            if (TailLength < 1) TailLength = 1;
            int TailStart = BufferHt - TailLength;
            if (TailStart < 1) TailStart = 1;
            for (int i = 0; i < Count; i++)
            {
                MeteorClass m = new MeteorClass();
                m.x = rand() % BufferWi;
                m.y = BufferHt - 1 - (rand() % TailStart);
                switch (MeteorType)
                {
                    case 1:
                        Palette.SetRangeColor(hsv0, hsv1, m.hsv);
                        break;
                    case 2:
                        m.hsv = Palette.GetHSV(rand() % colorcnt);
                        break;
                }
                meteors.Add(m);
            }

            // render meteors
            foreach (MeteorClass meteor in meteors)
            {
                {
                    for (int ph = 0; ph < TailLength; ph++)
                    {
                        switch (MeteorType)
                        {
                            case 0:
                                hsv.Hue = (float)(rand() % 1000) / 1000.0f;
                                hsv.Saturation = 1.0f;
                                hsv.Value = 1.0f;
                                break;
                            default:
                                hsv.SetToHSV(meteor.hsv);
                                break;
                        }
                        hsv.Value *= (float)(1.0 - (double)ph / TailLength);
                        SetPixel(meteor.x, meteor.y + ph, hsv);
                    }
                    meteor.y -= mspeed;
                }
            }
            // delete old meteors
            int meteorNum = 0;
            while (meteorNum < meteors.Count)
            {
                if (meteors[meteorNum].HasExpired(TailLength))
                {
                    meteors.RemoveAt(meteorNum);
                }
                else
                {
                    meteorNum++;
                }
            }
        }

        #endregion // Meteors


        // FIREWORKS IS BROKEN
        #region Fireworks

        //private RgbFireworks[] fireworkBursts = new RgbFireworks[20000];
        private List<RgbFireworks> fireworkBursts = null;

        private void InitFireworksBuffer()
        {
            if (fireworkBursts == null)
            {
                fireworkBursts = new List<RgbFireworks>();
                for (int burstNum = 0; burstNum < 20000; burstNum++)
                {
                    RgbFireworks firework = new RgbFireworks();
                    fireworkBursts.Add(firework);
                }
            }
        }

        public void RenderFireworks(int Number_Explosions, int Count, float Velocity, int Fade)
        {
            int idxFlakes = 0;
            int i = 0, x, y, mod100;
            int x25, x75, y25, y75, stateChunk, denom;
            const int maxFlakes = 1000;
            //float velocity = 3.5;
            int startX;
            int startY; //, ColorIdx;
            float v;
            HSV hsv = new HSV();
            Color color, rgbcolor;
            int colorcnt = GetColorCount();

            //Console.WriteLine(Number_Explosions + ":" + Count + ":" + Velocity + ":" + Fade);

            InitFireworksBuffer();

            if (State == 0)
            {
                for (i = 0; i < maxFlakes; i++)
                {
                    fireworkBursts[i]._bActive = false;
                }
            }
            denom = (101 - Number_Explosions) * 100;
            if (denom < 1) denom = 1;
            stateChunk = (int)State / denom;
            if (stateChunk < 1) stateChunk = 1;


            //mod100 = (int)(State % ((101 - Number_Explosions) * 10));
            mod100 = (int)(State % (101 - Number_Explosions) * 10);
            //Console.WriteLine("s:" + State + " ne:" + ((101 - Number_Explosions) * 10));
            //Console.WriteLine("mod100:" + mod100);
            //        mod100 = (int)(state/stateChunk);
            //        mod100 = mod100%10;
            if (mod100 == 0)
            {

                x25 = (int)(BufferWi * 0.25);
                x75 = (int)(BufferWi * 0.75);
                y25 = (int)(BufferHt * 0.25);
                y75 = (int)(BufferHt * 0.75);
                startX = (int)BufferWi / 2;
                startY = (int)BufferHt / 2;
                startX = x25 + rand() % (x75 - x25);
                startY = y25 + rand() % (y75 - y25);
                // turn off all bursts

                // Create new bursts
                for (i = 0; i < Count; i++)
                {
                    do
                    {
                        idxFlakes = (idxFlakes + 1) % maxFlakes;
                    }
                    while (fireworkBursts[idxFlakes]._bActive);
                    fireworkBursts[idxFlakes].Reset(startX, startY, true, Velocity);
                }
            }
            else
            {
                for (i = 0; i < maxFlakes; i++)
                {
                    // ... active flakes:
                    if (fireworkBursts[i]._bActive)
                    {
                        // Update position
                        fireworkBursts[i]._x += fireworkBursts[i]._dx;
                        fireworkBursts[i]._y += (float)(-fireworkBursts[i]._dy - fireworkBursts[i]._cycles * fireworkBursts[i]._cycles / 10000000.0);
                        // If this flake run for more than maxCycle, time to switch it off
                        fireworkBursts[i]._cycles += 20;
                        if (10000 == fireworkBursts[i]._cycles) // if (10000 == fireworkBursts[i]._cycles)
                        {
                            fireworkBursts[i]._bActive = false;
                            continue;
                        }
                        // If this flake hit the earth, time to switch it off
                        if (fireworkBursts[i]._y >= BufferHt)
                        {
                            fireworkBursts[i]._bActive = false;
                            continue;
                        }
                        // Draw the flake, if its X-pos is within frame
                        if (fireworkBursts[i]._x >= 0.0 && fireworkBursts[i]._x < BufferWi)
                        {
                            // But only if it is "under" the roof!
                            if (fireworkBursts[i]._y >= 0.0)
                            {
                                // sean we need to set color here
                            }
                        }
                        else
                        {
                            // otherwise it just got outside the valid X-pos, so switch it off
                            fireworkBursts[i]._bActive = false;
                            continue;
                        }
                    }
                }
            }
            //// Clear all Pixels
            //color = Color.Black;
            //hsv = HSV.ColorToHSV(color);
            //for (y = 0;
            //        y < BufferHt;
            //        y++)
            //{
            //    for (x = 0; x < BufferWi; x++)
            //    {
            //        SetPixel(x, y, hsv);
            //    }
            //}

            // Draw bursts with fixed color
            //    if(state%300<=300) rgbcolor = wxColour(0,255,0);
            //    if(state%300<=250) rgbcolor = wxColour(255,0,255);
            //    if(state%300<=150) rgbcolor = wxColour(255,255,0);
            //    if(state%300<=100) rgbcolor = wxColour(255,0,0);
            //    if(state%300<=50) rgbcolor = wxColour(255,255,255);

            if (mod100 == 0) rgbcolor = Color.FromArgb(0, 255, 255);
            else if (mod100 == 1) rgbcolor = Color.FromArgb(255, 0, 0);
            else if (mod100 == 2) rgbcolor = Color.FromArgb(0, 255, 0);
            else if (mod100 == 3) rgbcolor = Color.FromArgb(0, 0, 255);
            else if (mod100 == 4) rgbcolor = Color.FromArgb(255, 255, 0);
            else if (mod100 == 5) rgbcolor = Color.FromArgb(0, 255, 0);
            else rgbcolor = Color.White;
            hsv = HSV.ColorToHSV(rgbcolor);


            //ColorIdx=rand() % colorcnt; // Select random numbers from 0 up to number of colors the user has checked. 0-5 if 6 boxes checked
            //ColorIdx=0;
            //       palette.GetHSV(ColorIdx, hsv); // Now go and get the hsv value for this ColorIdx



            for (i = 0; i < 1000; i++)
            {
                if (fireworkBursts[i]._bActive == true)
                {
                    v = (float)(((Fade * 10.0) - fireworkBursts[i]._cycles) / (Fade * 10.0));
                    if (v < 0) v = 0.0f;

                    hsv.Value = v;
                    SetPixel((int)fireworkBursts[i]._x, (int)fireworkBursts[i]._y, hsv);
                }
            }
        }

        public class RgbFireworks
        {
            //static const float velocity = 2.5;
            const int maxCycle = 4096;
            const int maxNewBurstFlakes = 10;
            public float _x;
            public float _y;
            public float _dx;
            public float _dy;
            public float vel;
            public float angle;
            public bool _bActive;
            public int _cycles;
            private Random random = new Random();

            public void Reset(int x, int y, bool active, float velocity)
            {
                _x = x;
                _y = y;
                vel = (random.Next() - int.MaxValue / 2) * velocity / (int.MaxValue / 2);
                angle = (float)(2.0 * Math.PI * (double)random.Next() / (double)int.MaxValue);
                _dx = (float)(vel * Math.Cos(angle));
                _dy = (float)(vel * Math.Sin(angle));
                _bActive = active;
                _cycles = 0;

                //Console.WriteLine("vel:" + vel + " angle:" + angle + " _dx:" + _dx + " _dy:" + _dy);
            }
        }

        #endregion

        #endregion // Nutcracker Effects

        #region Pixels

        public void InitBuffer(int bufferWidth, int bufferHeight)
        {
            _pixels.Clear();
            _tempbuf.Clear();

            List<Color> column;
            for (int width = 0; width < bufferWidth; width++)
            {
                column = new List<Color>();
                _pixels.Add(column);
                column = new List<Color>();
                _tempbuf.Add(column);
                for (int height = 0; height < bufferHeight; height++)
                {
                    _pixels[width].Add(Color.Transparent);
                    _tempbuf[width].Add(Color.Transparent);
                }
            }

            Array.Resize(ref FireBuffer, bufferWidth * bufferHeight);
            Array.Resize(ref WaveBuffer0, bufferWidth * bufferHeight);
            Array.Resize(ref WaveBuffer1, bufferWidth * bufferHeight);
            Array.Resize(ref WaveBuffer2, bufferWidth * bufferHeight);

            InitFirePalette();

            State = 0;
        }

        // initialize FirePalette[]
        private void InitFirePalette()
        {
            HSV hsv = new HSV();
            Color color;

            FirePalette.Clear();
            //wxImage::HSVValue hsv;
            //wxImage::RGBValue rgb;
            //wxColour color;
            int i;
            // calc 100 reds, black to bright red
            hsv.Hue = 0.0f;
            hsv.Saturation = 1.0f;
            for (i = 0; i < 100; i++)
            {
                hsv.Value = (float)i / 100.0f;
                //rgb = wxImage::HSVtoRGB(hsv);
                //color.Set(rgb.red,rgb.green,rgb.blue);
                color = HSV.HSVtoColor(hsv);
                FirePalette.Add(color);
                //FirePalette.push_back(color);
            }

            // gives 100 hues red to yellow
            hsv.Value = 1.0f;
            for (i = 0; i < 100; i++)
            {
                //rgb = wxImage::HSVtoRGB(hsv);
                //color.Set(rgb.red,rgb.green,rgb.blue);
                color = HSV.HSVtoColor(hsv);
                //FirePalette.push_back(color);
                FirePalette.Add(color);
                hsv.Hue += 0.00166666f;
            }
        }

        // 0,0 is lower left
        public void SetPixel(int x, int y, Color color)
        {
            if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
            {
                _pixels[x][y] = color;
            }
        }

        // 0,0 is lower left
        public void SetPixel(int x, int y, HSV hsv)
        {
            Color color = HSV.HSVtoColor(hsv);
            //Console.WriteLine("h:" + hsv.Hue + " s:" + hsv.Saturation + " v:" + hsv.Value + " r:" + color.R + " g:" + color.G + " b:" + color.B + " a:" + color.A);
            //Console.WriteLine(" r:" + color.R + " g:" + color.G + " b:" + color.B + " a:" + color.A);
            SetPixel(x, y, color);
        }

        public Color GetPixel(int x, int y)
        {
            return _pixels[x][y];
        }

        public Color GetPixel(int pixelToGet)
        {
            Color color = Color.White;
            int pixelNum = 0;
            for (int x = 0; x < BufferWi; x++)
            {
                for (int y = 0; y < BufferHt; y++)
                {
                    if (pixelNum == pixelToGet)
                    {
                        return _pixels[x][y]; 
                    }
                    pixelNum++;
                }
            }
            return color;
        }

        public int PixelCount()
        {
            return BufferWi * BufferHt;
        }

        public void ClearPixels()
        {
            foreach (List<Color> column in Pixels)
            {
                for (int row = 0; row < column.Count; row++)
                {
                    column[row] = Color.Transparent;
                }
            }
        }

        // 0,0 is lower left
        private void SetTempPixel(int x, int y, Color color)
        {
            if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
            {
                //tempbuf[y*BufferWi+x]=color;
                _tempbuf[x][y] = color;
            }
        }

        // 0,0 is lower left
        private Color GetTempPixel(int x, int y)
        {
            if (x >= 0 && x < BufferWi && y >= 0 && y < BufferHt)
            {
                //return tempbuf[y*BufferWi+x];
                return _tempbuf[x][y];
            }
            return Color.Transparent;
        }

        void ClearTempBuf()
        {
            for (int x = 0; x < BufferWi; x++)
            {
                for (int y = 0; y < BufferHt; y++)
                {
                    _tempbuf[x][y] = Color.Black;
                }
            }
        }

        private void CopyTempBufToPixels()
        {
            for (int x = 0; x < BufferWi; x++)
            {
                for (int y = 0; y < BufferHt; y++)
                {
                    Pixels[x][y] = _tempbuf[x][y];
                }
            }
        }

        private void CopyPixelsToTempBuf()
        {
            for (int x = 0; x < BufferWi; x++)
            {
                for (int y = 0; y < BufferHt; y++)
                {
                    _tempbuf[x][y] = Pixels[x][y];
                }
            }
        }

#endregion

        public void RenderNextEffect(Effects effect)
        {
            ClearPixels();
            switch (effect) 
            {
                case Effects.Bars:
                    RenderBars(Data.Bars_PaletteRepeat, Data.Bars_Direction, Data.Bars_Highlight, Data.Bars_3D);
                    break;
                case Effects.Garlands:
                    RenderGarlands(Data.Garland_Type, Data.Garland_Spacing);
                    break;
                case Effects.Butterfly:
                    RenderButterfly(Data.Butterfly_Colors, Data.Butterfly_Style, Data.Butterfly_BkgrdChunks, Data.Butterfly_BkgrdSkip);
                    break;
                case Effects.ColorWash:
                    RenderColorWash(Data.ColorWash_FadeHorizontal, Data.ColorWash_FadeVertical, Data.ColorWash_Count);
                    break;
                case Effects.Fire:
                    RenderFire(Data.Fire_Height);
                    break;
                case Effects.Life:
                    RenderLife(Data.Life_CellsToStart, Data.Life_Type);
                    break;
                case Effects.Meteors:
                    RenderMeteors(Data.Meteor_Colors, Data.Meteor_Count, Data.Meteor_TrailLength);
                    break;
                case Effects.Fireworks:
                    RenderFireworks(Data.Fireworks_Explosions, Data.Fireworks_Particles, Data.Fireworks_Velocity, Data.Fireworks_Fade);
                    break;
            }
            SetNextState(false);
        }

    }
}
