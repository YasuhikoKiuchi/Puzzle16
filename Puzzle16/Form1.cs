using System.Diagnostics;

namespace Puzzle16
{
    public partial class Form1 : Form
    {
        #region フィールド

        private readonly static Random random = new(); // 乱数生成用

        private readonly int[,] cells = new int[4, 4]; // セル
        //private readonly int[,] cells = new int[16, 8]; // アイキャッチ画像用

        private Bitmap[] bmps = new Bitmap[16]; // ピース画像格納用

        private PictureBox pictureBox2 = new(); // ピースのネタを描くためのピクチャーボックス

        private System.Windows.Forms.Timer timClock = new(); // 時計を再描画するためのタイマー

        #endregion

        #region 初期処理

        public Form1() // コンストラクタ
        {
            InitializeComponent();

            // アイキャッチ画像用
            //SetBounds(0, 0, 16 * 64 + 16, 64 * 8 + 36, BoundsSpecified.Size);
            //pictureBox1.Dock = DockStyle.Fill;
            //FormBorderStyle = FormBorderStyle.None;

            //PreparePiece(); // 絵バージョンのピース準備

            SetupPictureBox2AndTimer(); // 時計バージョンのピース準備

            SetupCells(); // ピースを順番に並べる
            Shuffle(); // シャッフル
            Refresh(); // 16パズル再描画
        }

        private void SetupCells() // 準備
        {
            for (int x = 0; x < cells.GetLength(0); x++) // ピースを並べる
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    cells[x, y] = y * cells.GetLength(0) + x + 1;
                }
            }
            cells[3, 3] = 0; // 一番右下のピースをなくす
            //cells[15, 7] = 0; // 一番右下のピースをなくす // アイキャッチ画像用
        }

        private void Shuffle() // シャッフル
        {
            for (int i = 0; i < 5000; i++)
            {
                int x = random.Next(0, 4);
                int y = random.Next(0, 4);
                MovePiece(x, y);
            }
        }

        #endregion

        #region === 絵バージョン ===

        private void PreparePiece() // ピース準備
        {
            Bitmap? srcBitmap = Image.FromFile("16puzzle_bg_sample.jpg") as Bitmap;
            srcBitmap?.SetResolution(96, 96);

            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    Rectangle srcRect = new Rectangle(x * 64, y * 64, 63, 63);
                    bmps[y * 4 + x] = srcBitmap!.Clone(srcRect, srcBitmap.PixelFormat);
                }
            }
        }

        #endregion

        #region === 時計バージョン ===

        private void SetupPictureBox2AndTimer()
        {
            // pictureBox2の準備
            pictureBox2.SetBounds(0, 0, 256, 256, BoundsSpecified.Size);
            pictureBox2.BackColor = Color.Black;
            pictureBox2.Paint += pictureBox2_Paint;
            pictureBox2.Refresh();

            // タイマーの準備
            timClock.Tick += timClock_Tick!;
            timClock.Interval = 1000;
            timClock.Start();

            // ピース作成
            PreparePiece2();
        }

        private void timClock_Tick(object sender, EventArgs e) // 時計更新タイマーTICKイベント処理
        {
            pictureBox2.Refresh(); // 時計を再描画する
            PreparePiece2(); // ピースを作る
            pictureBox1.Refresh(); // 16パズルを再描画する
        }

        private void pictureBox2_Paint(object? sender, PaintEventArgs e) // 時計描画イベントメソッド
        {
            e.Graphics.DrawEllipse(Pens.White, 16, 16, 224, 224);

            var t = DateTime.Now;
            var dt = Math.PI / 30; // (=6.28/60)
            double theta;
            int cx = 128;
            int cy = 128;

            // 目盛りを描く
            for (int i = 0; i < 60; i += 5)
            {
                theta = i * dt - Math.PI / 2D;
                double x1 = Math.Cos(theta) * (i % 10 == 0 ? 95 : 100) + cx;
                double y1 = Math.Sin(theta) * (i % 10 == 0 ? 95 : 100) + cy;
                double x2 = Math.Cos(theta) * 110 + cx;
                double y2 = Math.Sin(theta) * 110 + cy;
                e.Graphics.DrawLine(Pens.White, (int)x1, (int)y1, (int)x2, (int)y2);
            }

            // 時針
            theta = (Math.PI / 6) * (12 - (t.Hour % 12)) + Math.PI / 2;
            double xx2 = Math.Cos(theta) * 60 + cx;
            double yy2 = -Math.Sin(theta) * 60 + cy;
            e.Graphics.DrawLine(Pens.Blue, cx, cy, (int)xx2, (int)yy2);

            // 分針
            theta = dt * (60 - t.Minute) + Math.PI / 2;
            xx2 = Math.Cos(theta) * 80 + cx;
            yy2 = -Math.Sin(theta) * 80 + cy;
            e.Graphics.DrawLine(Pens.Red, cx, cy, (int)xx2, (int)yy2);

            // 秒針
            theta = dt * (60 - t.Second) + Math.PI / 2;
            xx2 = Math.Cos(theta) * 100 + cx;
            yy2 = -Math.Sin(theta) * 100 + cy;
            e.Graphics.DrawLine(Pens.Green, cx, cy, (int)xx2, (int)yy2);
        }

        private void PreparePiece2() // ピース準備
        {
            Bitmap srcBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox2.DrawToBitmap(srcBitmap, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));

            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    Rectangle srcRect = new Rectangle(x * 64, y * 64, 63, 63);
                    bmps[y * 4 + x] = srcBitmap.Clone(srcRect, srcBitmap.PixelFormat);
                }
            }
        }

        #endregion

        #region === 字バージョン ===

        //private void pictureBox2_Paint(object? sender, PaintEventArgs e)
        //{
        //    e.Graphics.DrawString("技", new Font("MS ゴシック", 140), Brushes.White, 0, 32);
        //}

        #endregion

        #region アイキャッチ画像用

        public static (int R, int G, int B) GeneratePastelColor()
        {
            int r = random.Next(200, 256);
            int g = random.Next(200, 256);
            int b = random.Next(200, 256);

            return (r, g, b);
        }

        public static string RGBToHex(int r, int g, int b)
        {
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        #endregion

        #region 描画処理

        private void pictureBox1_Paint(object sender, PaintEventArgs e) // ピクチャーボックス描画イベント
        {
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y] != 0)
                    {
                        // 通常
                        //e.Graphics.FillRectangle(Brushes.AliceBlue, x * 64, y * 64, 63, 63);
                        //e.Graphics.DrawString(cells[x, y].ToString("D2"), new Font("MS ゴシック", 32), Brushes.Black, x * 64, y * 64 + 8);

                        // アイキャッチ画像用
                        //int r, g, b;
                        //(r, g, b) = GeneratePastelColor();
                        //Color color = Color.FromArgb(r, g, b);
                        //Brush br = new SolidBrush(color);
                        //e.Graphics.FillRectangle(br, x * 64, y * 64, 63, 63);
                        //e.Graphics.DrawString(cells[x, y].ToString("D3"), new Font("MS ゴシック", 12), Brushes.Gray, x * 64 + 14, y * 64 + 22);

                        // 絵バージョン・時計バージョン
                        e.Graphics.DrawImage(bmps[cells[x, y] - 1], new Point(x * 64, y * 64));
                    }
                }
            }
        }

        #endregion

        #region ピースを動かす処理・判定処理

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e) // マウスクリック時処理
        {
            int x = e.X / 64;
            int y = e.Y / 64;
            MovePiece(x, y);
            if (Judge()) // 判定処理の呼び出しを追加
            {
                MessageBox.Show("Congratulations!"); // できていたらメッセージ表示
                SetupCells(); // メッセージダイアログのOKボタンを押したら、次のゲーム開始
                Refresh();
            }
        }

        private void MovePiece(int x, int y) // ピースを動かす
        {
            if (cells[x, y] == 0) return; // 何もないところをクリックしたら何もせずreturn

            if (x > 0 && cells[x - 1, y] == 0)
            {
                cells[x - 1, y] = cells[x, y];
                cells[x, y] = 0;
            }
            else if (x < 3 && cells[x + 1, y] == 0)
            {
                cells[x + 1, y] = cells[x, y];
                cells[x, y] = 0;
            }
            else if (y > 0 && cells[x, y - 1] == 0)
            {
                cells[x, y - 1] = cells[x, y];
                cells[x, y] = 0;
            }
            else if (y < 3 && cells[x, y + 1] == 0)
            {
                cells[x, y + 1] = cells[x, y];
                cells[x, y] = 0;
            }

            Refresh();
        }

        private bool Judge() // そろっているか判定する
        {
            bool done = true;

            for (int y = 0; y < cells.GetLength(0); y++)
            {
                for (int x = 0; x < cells.GetLength(1); x++)
                {
                    //Debug.Print("{0},{1}={2} (=?{3})", x, y, cells[x,y], y * 4 + x + 1) ;

                    if (x == 3 && y == 3) // 一番右下については「0かどうか」を確認する
                    {
                        if (cells[x, y] != 0)
                        {
                            done = false;
                            break;
                        }
                    }
                    else if (cells[x, y] != y * 4 + x + 1) // それ以外は所定の値になっているか確認する
                    {
                        done = false;
                        break;
                    }
                }
            }

            return done;
        }

        #endregion
    }
}