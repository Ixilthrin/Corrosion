using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CorrosionApp
{
    public partial class MainWindow : Window
    {
        List<BallBounce> _balls;
        DispatcherTimer _frameTimer;

        public MainWindow()
        {
            InitializeComponent();
            _balls = new List<BallBounce>();
            _frameTimer = new DispatcherTimer();
        }

        private BallBounce CreateBall(Rect pBoundary, Random pRand)
        {
            float posX = (float)(pBoundary.X + pRand.NextDouble() * pBoundary.Width);
            float posY = (float)(pBoundary.Y + pRand.NextDouble() * pBoundary.Height);
            Point position = new Point(posX, posY);
            float velX = (float)(pRand.NextDouble() * 6 - 2.4);
            float velY = (float)(pRand.NextDouble() * 6 - 2.4);
            Point velocity = new Point(velX, velY);
            float radius = 2;
            return new BallBounce(pBoundary, position, velocity, radius);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int ballCount = 100;
            
            Rect boundary = new Rect(0, 0, 500, 500);
            Random rand = new Random();

            for (int i = 0; i < ballCount; ++i)
            {
                _balls.Add(CreateBall(boundary, rand));
            }
            
            BallCollection collection = new BallCollection(_balls);
            
            bounceCanvas.Children.Add(collection);

            int fps = 60;
            _frameTimer.Tick += time_Tick;
            _frameTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / fps);
            _frameTimer.Start();

        }

        void time_Tick(object sender, EventArgs e)
        {
            foreach (var ball in _balls)
            {
                ball.Update();
            }
        }
    }
    public class BallCollection : FrameworkElement
    {
        private DrawingGroup _drawingGroup;

        public BallCollection(List<BallBounce> particles)
        {
            _drawingGroup = new DrawingGroup();

            foreach (var ball in particles)
            {
                var geometry = ball.Ball;

                Brush brush = Brushes.White;
                brush.Freeze();

                var drawing = new GeometryDrawing(brush, null, geometry);

                _drawingGroup.Children.Add(drawing);
            }

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawDrawing(_drawingGroup);
        }
    }
    public class BouncePhysics
    {
        protected Point _location;
        protected Point _velocity;
        protected Rect _boundary;

        public BouncePhysics(Rect pBoundary, Point pLocation, Point pVelocity)
        {
            _boundary = pBoundary;

            _location = pLocation;
            _velocity = pVelocity;
        }

        public double X
        {
            get
            {
                return _location.X;
            }
        }

        public double Y
        {
            get
            {
                return _location.Y;
            }
        }

        public virtual void Update()
        {
            UpdatePosition();
            CheckBoundary();
        }

        private void UpdatePosition()
        {
            _location.X += _velocity.X;
            _location.Y += _velocity.Y;
        }

        private void CheckBoundary()
        {
            if (_location.X > _boundary.Width + _boundary.X)
            {
                _velocity.X = -_velocity.X;
                _location.X = _boundary.Width + _boundary.X;
            }

            if (_location.X < _boundary.X)
            {
                _velocity.X = -_velocity.X;
                _location.X = _boundary.X;
            }

            if (_location.Y > _boundary.Height + _boundary.Y)
            {
                _velocity.Y = -_velocity.Y;
                _location.Y = _boundary.Height + _boundary.Y;
            }

            if (_location.Y < _boundary.Y)
            {
                _velocity.Y = -_velocity.Y;
                _location.Y = _boundary.Y;
            }
        }
    }

    public class BallBounce : BouncePhysics
    {
        protected EllipseGeometry _ball;

        public BallBounce(Rect pBoundary, Point pPosition, Point pVelocity, float pRadius)
            : base(pBoundary, pPosition, pVelocity)
        {
            _ball = new EllipseGeometry(pPosition, pRadius, pRadius);
        }

        public EllipseGeometry Ball
        {
            get
            {
                return _ball;
            }
        }

        public override void Update()
        {
            base.Update();
            _ball.Center = _location;
        }
    }
}
