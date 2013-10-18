using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using System.IO;

namespace Snake
{
    public class GameStick
    {
        #region Constants
        private const int WAITING = 0;
        private const int START_LEFT_TURN = 1;
        private const int START_RIGHT_TURN = 2;
        private const int START_NEW_GAME = 3;
        private const int RETURNING = 5;

        public const int TURN_LEFT = 99;
        public const int TURN_RIGHT = 100;
        public const int START_GAME = 101;
        #endregion

        #region Variables
        private Label xLabel, yLabel, zLabel, orientationLabel; 
        private Timer timer; 
        private SerialPort port;
        private GameController game; 

        private int _state = 0;
        private int _ticksInState = 0;

        private int xByte = 0;
        private int yByte = 0;
        private int zByte = 0;

        private int xSum = 0;
        private int ySum = 0;
        private int zSum = 0;
        private int bytesPerSegment = 0;

        private int[] xAccel = new int[] { 0, 0 };
        private int[] yAccel = new int[] { 0, 0 };
        private int[] zAccel = new int[] { 0, 0 };

        private string orientation;
        bool displayUI = false;

        public bool demoMode { get; set; }
        #endregion

        #region Constructor
        public GameStick(GameController _game)
        {
            game = _game;
            demoMode = false;

            port = new SerialPort();
            port.BaudRate = 128000;
            port.DataReceived += port_DataReceived;

            timer = new Timer();
            timer.Interval = 20;
            timer.Tick += new EventHandler(tick);
            timer.Start();
        }
        #endregion

        #region IO
        public bool isConnected() { return port.IsOpen; }
        public bool OpenPort(string portName)
        {
            try
            {
                port.PortName = portName;
                port.Open();
                MessageBox.Show("Successfully Connected To GameStick!");
                return true;
            }
            catch (System.InvalidOperationException)
            {
                MessageBox.Show("Already Attached to GameStick!");
            }
            catch (Exception i)
            {
                MessageBox.Show(i.GetType().ToString());
            }
            return false;
        }
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (port.ReadBufferSize > 0 && port.IsOpen)
                {
                    readNextBytes();
                    processBytes();
                }
            }
            catch (IOException)
            {
                game.pause();
                MessageBox.Show("System Timed Out! Please ensure that the network cable is attached");
            }
        }
        private void readNextBytes()
        {
            if (port.ReadByte() == 255)
            {
                xByte = port.ReadByte();
                yByte = port.ReadByte();
                zByte = port.ReadByte();
            }
        }
        #endregion

        #region State Logic
        private void processState()
        {
            if (game.isRunning())
            {
                switch (_state)
                {
                    case TURN_RIGHT:
                    case TURN_LEFT:
                        return;
                    case RETURNING:
                        if (yAccel[0] > 100 && yAccel[0] < 170 &&
                            zAccel[0] > 100 && zAccel[0] < 170)
                        {
                            _state = WAITING;
                        }
                        break;
                    case START_RIGHT_TURN:
                        if (zAccel[0] > 170)
                        {
                            _state = TURN_RIGHT;
                        }
                        break;
                    case START_LEFT_TURN:
                        if (yAccel[0] > 170)
                        {
                            _state = TURN_LEFT;
                        }
                        break;
                    default:
                        if (yAccel[0] < 80 || zAccel[0] < 80)
                        {
                            
                            if (yAccel[0] < zAccel[0])
                            {
                                _state = START_LEFT_TURN;
                            }
                            else
                            {
                                _state = START_RIGHT_TURN;
                            }
                        }
                        _ticksInState = 0;
                        break;
                }
            }
            else
            {
                if (_state == START_NEW_GAME )
                {
                    if ( xAccel[0] > 80 && xAccel[0] < 110 )
                    {
                        game.start();
                    }
                }
                else if (xAccel[0] > 140 && yAccel[0] < 90 && zAccel[0] > 140)
                {
                    _state = START_NEW_GAME;
                }
            }

            
        }
        public void resetState() { _state = RETURNING; _ticksInState = 0; }

        private const int firstStepDoubleKick = 10;
        private const int secondStepDoubleKick = 11;
        private const int Threshold = 200;

        private void processDemoModeState()
        {
            switch (_state)
            {
                case firstStepDoubleKick:
                    if (xAccel[0] > 90 && xAccel[0] < 150 &&
                        yAccel[0] > 90 && yAccel[0] < 160 &&
                        zAccel[0] > 180)
                    {
                        System.Diagnostics.Debug.Write("second step double kick!");
                        _state = secondStepDoubleKick;
                    }
                    break;
                case secondStepDoubleKick:
                    if (xAccel[0] < 90)
                    {
                        MessageBox.Show("Wow you did a flippin double kick!");
                        _state = WAITING;
                    }
                    break;
                default:
                    // double jump kick
                    if (//xAccel[0] > 110 && xAccel[0] < 130 &&
                        yAccel[0] > 90 && yAccel[0] < 160 &&
                        zAccel[0] < 130)
                    {
                        System.Diagnostics.Debug.Write("first step double kick!");
                        _state = firstStepDoubleKick;
                    }
                    else if (xAccel[0] < 100 &&
                        yAccel[0] > 100 && yAccel[0] < 130 &&
                        zAccel[0] > 150 && zAccel[0] < 170)
                    {
                        MessageBox.Show("Wow you did a punch!");
                    }
                    break;

            }
        }

        private void determineOrientation()
        {
            if (xAccel[0] > 90 && xAccel[0] < 110 &&
                yAccel[0] > 110 && yAccel[0] < 130 &&
                zAccel[0] > 120 && zAccel[0] < 140)
            {
                orientation = "Up";
            }
            else if (xAccel[0] > 150 && xAccel[0] < 170 &&
                yAccel[0] > 120 && yAccel[0] < 140 &&
                zAccel[0] > 120 && zAccel[0] < 140)
            {
                orientation = "Down";
            }
            else if (xAccel[0] > 110 && xAccel[0] < 130 &&
                yAccel[0] > 110 && yAccel[0] < 130 &&
                zAccel[0] > 150 && zAccel[0] < 170)
            {
                orientation = "Face Up";
            }
            else if (xAccel[0] > 110 && xAccel[0] < 130 &&
                yAccel[0] > 150 && yAccel[0] < 170 &&
                zAccel[0] > 110 && zAccel[0] < 130)
            {
                orientation = "Rotate Left";
            }
            else if (xAccel[0] > 120 && xAccel[0] < 140 &&
                yAccel[0] > 80 && yAccel[0] < 100 &&
                zAccel[0] > 125 && zAccel[0] < 145)
            {
                orientation = "Rotate Right";
            }
            else if (xAccel[0] > 115 && xAccel[0] < 135 &&
                yAccel[0] > 120 && yAccel[0] < 140 &&
                zAccel[0] > 80 && zAccel[0] < 100)
            {
                orientation = "Face Down";
            }
        }
        #endregion

        #region Signal Processing
        private void processBytes()
        {
            xSum += xByte;
            ySum += yByte;
            zSum += zByte;

            bytesPerSegment++;
        }

        private void smoothPoints()
        {
            xAccel[1] = xAccel[0];
            yAccel[1] = yAccel[0];
            zAccel[1] = zAccel[0];

            xAccel[0] = xSum / bytesPerSegment;
            yAccel[0] = ySum / bytesPerSegment;
            zAccel[0] = zSum / bytesPerSegment;
            
            xSum = 0; ySum = 0; zSum = 0;
            bytesPerSegment = 0;
        }
        #endregion
        int timeInState;
        private void tick(object sender, EventArgs e)
        {
            if (port.IsOpen && bytesPerSegment > 0)
            {
                _ticksInState++;
                smoothPoints();
                processState();
                /*System.Diagnostics.Debug.Write("xAccel " + xAccel[0]
                               + " yAccel " + yAccel[0]
                                + " zAccel " + zAccel[0]
                                + " state: " + _state
                                + "\n");
                */

                if (demoMode)
                {
                    processDemoModeState();
                    timeInState = 1000;
                }
                else
                {
                    timeInState = 100;
                }

                if (_state == TURN_LEFT || _state == TURN_RIGHT)
                {
                    game.updateDirection(_state);
                }

                if (displayUI) 
                {
                    determineOrientation();
                    xLabel.Text = xAccel[0].ToString();
                    yLabel.Text = yAccel[0].ToString();
                    zLabel.Text = zAccel[0].ToString();
                    orientationLabel.Text = orientation; 
                }

                if (_ticksInState > timeInState)
                {
                    _state = WAITING;
                    _ticksInState = 0;
                }
            }
        }

        public void stopDisplayingText()
        {
            xLabel = null;
            yLabel = null;
            zLabel = null;
            displayUI = false;
        }

        public void displayText(Label x, Label y, Label z, Label orientation)
        {
            xLabel = x;
            yLabel = y;
            zLabel = z;
            orientationLabel = orientation;
            displayUI = true;
        }
    }
}
