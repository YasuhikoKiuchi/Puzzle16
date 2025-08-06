using System.Diagnostics;

namespace Puzzle16
{
    public partial class Form1 : Form
    {
        #region �t�B�[���h

        private readonly static Random random = new(); // ���������p

        private readonly int[,] cells = new int[4, 4]; // �Z��
        //private readonly int[,] cells = new int[16, 8]; // �A�C�L���b�`�摜�p

        private Bitmap[] bmps = new Bitmap[16]; // �s�[�X�摜�i�[�p

        private PictureBox pictureBox2 = new(); // �s�[�X�̃l�^��`�����߂̃s�N�`���[�{�b�N�X

        private System.Windows.Forms.Timer timClock = new(); // ���v���ĕ`�悷�邽�߂̃^�C�}�[

        #endregion

        #region ��������

        public Form1() // �R���X�g���N�^
        {
            InitializeComponent();

            // �A�C�L���b�`�摜�p
            //SetBounds(0, 0, 16 * 64 + 16, 64 * 8 + 36, BoundsSpecified.Size);
            //pictureBox1.Dock = DockStyle.Fill;
            //FormBorderStyle = FormBorderStyle.None;

            //PreparePiece(); // �G�o�[�W�����̃s�[�X����

            SetupPictureBox2AndTimer(); // ���v�o�[�W�����̃s�[�X����

            SetupCells(); // �s�[�X�����Ԃɕ��ׂ�
            Shuffle(); // �V���b�t��
            Refresh(); // 16�p�Y���ĕ`��
        }

        private void SetupCells() // ����
        {
            for (int x = 0; x < cells.GetLength(0); x++) // �s�[�X����ׂ�
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    cells[x, y] = y * cells.GetLength(0) + x + 1;
                }
            }
            cells[3, 3] = 0; // ��ԉE���̃s�[�X���Ȃ���
            //cells[15, 7] = 0; // ��ԉE���̃s�[�X���Ȃ��� // �A�C�L���b�`�摜�p
        }

        private void Shuffle() // �V���b�t��
        {
            for (int i = 0; i < 5000; i++)
            {
                int x = random.Next(0, 4);
                int y = random.Next(0, 4);
                MovePiece(x, y);
            }
        }

        #endregion

        #region === �G�o�[�W���� ===

        private void PreparePiece() // �s�[�X����
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

        #region === ���v�o�[�W���� ===

        private void SetupPictureBox2AndTimer()
        {
            // pictureBox2�̏���
            pictureBox2.SetBounds(0, 0, 256, 256, BoundsSpecified.Size);
            pictureBox2.BackColor = Color.Black;
            pictureBox2.Paint += pictureBox2_Paint;
            pictureBox2.Refresh();

            // �^�C�}�[�̏���
            timClock.Tick += timClock_Tick!;
            timClock.Interval = 1000;
            timClock.Start();

            // �s�[�X�쐬
            PreparePiece2();
        }

        private void timClock_Tick(object sender, EventArgs e) // ���v�X�V�^�C�}�[TICK�C�x���g����
        {
            pictureBox2.Refresh(); // ���v���ĕ`�悷��
            PreparePiece2(); // �s�[�X�����
            pictureBox1.Refresh(); // 16�p�Y�����ĕ`�悷��
        }

        private void pictureBox2_Paint(object? sender, PaintEventArgs e) // ���v�`��C�x���g���\�b�h
        {
            e.Graphics.DrawEllipse(Pens.White, 16, 16, 224, 224);

            var t = DateTime.Now;
            var dt = Math.PI / 30; // (=6.28/60)
            double theta;
            int cx = 128;
            int cy = 128;

            // �ڐ����`��
            for (int i = 0; i < 60; i += 5)
            {
                theta = i * dt - Math.PI / 2D;
                double x1 = Math.Cos(theta) * (i % 10 == 0 ? 95 : 100) + cx;
                double y1 = Math.Sin(theta) * (i % 10 == 0 ? 95 : 100) + cy;
                double x2 = Math.Cos(theta) * 110 + cx;
                double y2 = Math.Sin(theta) * 110 + cy;
                e.Graphics.DrawLine(Pens.White, (int)x1, (int)y1, (int)x2, (int)y2);
            }

            // ���j
            theta = (Math.PI / 6) * (12 - (t.Hour % 12)) + Math.PI / 2;
            double xx2 = Math.Cos(theta) * 60 + cx;
            double yy2 = -Math.Sin(theta) * 60 + cy;
            e.Graphics.DrawLine(Pens.Blue, cx, cy, (int)xx2, (int)yy2);

            // ���j
            theta = dt * (60 - t.Minute) + Math.PI / 2;
            xx2 = Math.Cos(theta) * 80 + cx;
            yy2 = -Math.Sin(theta) * 80 + cy;
            e.Graphics.DrawLine(Pens.Red, cx, cy, (int)xx2, (int)yy2);

            // �b�j
            theta = dt * (60 - t.Second) + Math.PI / 2;
            xx2 = Math.Cos(theta) * 100 + cx;
            yy2 = -Math.Sin(theta) * 100 + cy;
            e.Graphics.DrawLine(Pens.Green, cx, cy, (int)xx2, (int)yy2);
        }

        private void PreparePiece2() // �s�[�X����
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

        #region === ���o�[�W���� ===

        //private void pictureBox2_Paint(object? sender, PaintEventArgs e)
        //{
        //    e.Graphics.DrawString("�Z", new Font("MS �S�V�b�N", 140), Brushes.White, 0, 32);
        //}

        #endregion

        #region �A�C�L���b�`�摜�p

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

        #region �`�揈��

        private void pictureBox1_Paint(object sender, PaintEventArgs e) // �s�N�`���[�{�b�N�X�`��C�x���g
        {
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y] != 0)
                    {
                        // �ʏ�
                        //e.Graphics.FillRectangle(Brushes.AliceBlue, x * 64, y * 64, 63, 63);
                        //e.Graphics.DrawString(cells[x, y].ToString("D2"), new Font("MS �S�V�b�N", 32), Brushes.Black, x * 64, y * 64 + 8);

                        // �A�C�L���b�`�摜�p
                        //int r, g, b;
                        //(r, g, b) = GeneratePastelColor();
                        //Color color = Color.FromArgb(r, g, b);
                        //Brush br = new SolidBrush(color);
                        //e.Graphics.FillRectangle(br, x * 64, y * 64, 63, 63);
                        //e.Graphics.DrawString(cells[x, y].ToString("D3"), new Font("MS �S�V�b�N", 12), Brushes.Gray, x * 64 + 14, y * 64 + 22);

                        // �G�o�[�W�����E���v�o�[�W����
                        e.Graphics.DrawImage(bmps[cells[x, y] - 1], new Point(x * 64, y * 64));
                    }
                }
            }
        }

        #endregion

        #region �s�[�X�𓮂��������E���菈��

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e) // �}�E�X�N���b�N������
        {
            int x = e.X / 64;
            int y = e.Y / 64;
            MovePiece(x, y);
            if (Judge()) // ���菈���̌Ăяo����ǉ�
            {
                MessageBox.Show("Congratulations!"); // �ł��Ă����烁�b�Z�[�W�\��
                SetupCells(); // ���b�Z�[�W�_�C�A���O��OK�{�^������������A���̃Q�[���J�n
                Refresh();
            }
        }

        private void MovePiece(int x, int y) // �s�[�X�𓮂���
        {
            if (cells[x, y] == 0) return; // �����Ȃ��Ƃ�����N���b�N�����牽������return

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

        private bool Judge() // ������Ă��邩���肷��
        {
            bool done = true;

            for (int y = 0; y < cells.GetLength(0); y++)
            {
                for (int x = 0; x < cells.GetLength(1); x++)
                {
                    //Debug.Print("{0},{1}={2} (=?{3})", x, y, cells[x,y], y * 4 + x + 1) ;

                    if (x == 3 && y == 3) // ��ԉE���ɂ��Ắu0���ǂ����v���m�F����
                    {
                        if (cells[x, y] != 0)
                        {
                            done = false;
                            break;
                        }
                    }
                    else if (cells[x, y] != y * 4 + x + 1) // ����ȊO�͏���̒l�ɂȂ��Ă��邩�m�F����
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