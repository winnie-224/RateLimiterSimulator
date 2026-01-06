using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using RateLimiterSimulator.RateLimiting;
using RateLimiterSimulator.Services;

namespace RateLimiterSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
   
    public partial class MainWindow : Window
    {
        //System components
        private IRateLimiter _currentRateLimiter;

        private TokenBucketRateLimiter _tokenBucketLimiter;
        private SlidingWindowRateLimiter _slidingWindowLimiter;

        private ApiGateway _apiGateway;

        //Request Metrics
        private int _totalRequests = 0;
        private int _allowedRequests = 0;
        private int _blockedRequests = 0;
        //Update UI
        private readonly DispatcherTimer _uiRefreshTimer;
        //Configuration
        private const int BucketCapacity = 10;
        private const int RefillRatePerSecond = 1;

        //Consturctor
        public MainWindow()
        {
            InitializeComponent();
            //Initialize ratelimiter

            //Requests based on number of token : Capacity based
            _tokenBucketLimiter = new TokenBucketRateLimiter(
                capacity:10,
                refillRatePerSecond:1);
            //Requests restricted to window size: Time based
            _slidingWindowLimiter = new SlidingWindowRateLimiter(
                maxRequests: 5,
                windowSize: TimeSpan.FromSeconds(5)
                );

            _currentRateLimiter = _tokenBucketLimiter;
            _apiGateway = new ApiGateway(_currentRateLimiter);

            //Initialize UI
            UpdateDecisionUI("No request", Brushes.LightGray);
            UpdateMetricsUI();
            UpdateTokenUI(BucketCapacity);
            
            //UI refresh
            _uiRefreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _uiRefreshTimer.Tick += RefreshTokenUI;
            _uiRefreshTimer.Start();
        }
        //====Button Handlers====

        private void SendRequest_Click(object sender, RoutedEventArgs e)
        {
            HandleRequest();
        }
        private void SendBurst_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i<10; i++)
            {
                HandleRequest();
            }
        }
        //====Request Flow====
        private void HandleRequest()
        {
            _totalRequests++;
            string clientId = GetClientId();
            bool allowed = _apiGateway.HandleRequest(clientId);
            if (allowed)
            {
                _allowedRequests++;
                UpdateDecisionUI("Request Allowed", Brushes.LightGreen);
            }
            else
            {
                _blockedRequests++;
                UpdateDecisionUI("Rate Limit Exceeded", Brushes.IndianRed);
            }
            UpdateMetricsUI();
        }
        //====UI Update Methods====
        private void RefreshTokenUI(object? sender, EventArgs e)
        {
            //Ask limiter for current token count per client
            string clientId = GetClientId();
            if(_currentRateLimiter is TokenBucketRateLimiter tokenLimiter)
            {
                int tokens = tokenLimiter.GetAvailableTokens(clientId);
                UpdateTokenUI(tokens);
            }
            else
            {   //Time based sliding window does not have tokens
                TokenProgressBar.Value = 0;
                TokenCountText.Text = "N/A (Sliding Window)";
            }

        }
        private void UpdateTokenUI(int tokens)
        {
            TokenProgressBar.Maximum = BucketCapacity;
            TokenProgressBar.Value = tokens;
            TokenCountText.Text = $"{tokens}/{BucketCapacity}";
        }
        private void UpdateMetricsUI()
        {
            TotalRequestsText.Text = $"Total Requests:{_totalRequests}";
            AllowedRequestsText.Text = $"Allowed Requests:{_allowedRequests}";
            BlockedRequestsText.Text = $"Blocked Requests:{_blockedRequests}";

            BucketCapacityText.Text = $"Bucket Capacity:{BucketCapacity}";
            RefillRateText.Text = $"Refill Rate:{RefillRatePerSecond} token/sec";
        }
        private void UpdateDecisionUI(string message, Brush color)
        {
            DecisionText.Text = message;
            DecisionIndicator.Background = color;
        }
        private string GetClientId()
        {
            return string.IsNullOrWhiteSpace(ClientIdBox.Text)
                ? "default-client"
                : ClientIdBox.Text.Trim();

        }


        private void AlgorithmSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
          {

            if(DecisionText == null || DecisionIndicator == null)
            {
                return;
            }
            
            if (AlgorithmSelector.SelectedItem is ComboBoxItem selected)
            {
                string algorithm = selected.Content.ToString();

                if (algorithm == "Token Bucket")
                {
                    _currentRateLimiter = _tokenBucketLimiter;
                    UpdateDecisionUI("Switched to Token Bucket", Brushes.LightBlue);
                }
                else if (algorithm == "Sliding Window")
                {
                    _currentRateLimiter = _slidingWindowLimiter;
                    UpdateDecisionUI("Switched to Sliding Window", Brushes.LightYellow);
                }

                _apiGateway = new ApiGateway(_currentRateLimiter);
            }
        }

    }
}