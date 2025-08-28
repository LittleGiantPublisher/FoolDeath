using System;

namespace F
{
    public static class CombatStateStatus
    {
        private static CombatPhase _currentPhase = CombatPhase.Neutral;
        private static int _round = 1;
        private static bool _phaseStarted = false;
        private static int _karma = 0;
        private static int _minKarma = -50;
        private static int _maxKarma = 50;
        private static int _money = 0;
        private static int _gamePoints = 0; 
        private static int _totalCoins = 0; 

        public static bool[] _coinsBought = new bool[3] { false, false, false };

        public static event EventHandler<CombatPhase> PhaseChangedEvent;
        public static event EventHandler<int> RoundChangedEvent;
        public static event EventHandler<int> KarmaChangedEvent;
        public static event EventHandler<int> MinKarmaChangedEvent;
        public static event EventHandler<int> MaxKarmaChangedEvent;
        public static event EventHandler<int> MoneyChangedEvent;
        public static event EventHandler<int> GamePointsChangedEvent; 
        
        public static CombatPhase CurrentPhase
        {
            get => _currentPhase;
            set
            {
                if (_currentPhase != value)
                {
                    _currentPhase = value;
                    PhaseChangedEvent?.Invoke(typeof(CombatStateStatus), _currentPhase);
                }
            }
        }

        public static int Round
        {
            get => _round;
            set
            {
                if (_round != value)
                {
                    _round = value;
                    RoundChangedEvent?.Invoke(typeof(CombatStateStatus), _round);
                }
            }
        }

        public static int Karma
        {
            get => _karma;
            set
            {
                if (_karma != value)
                {
                    _karma = value;
                    KarmaChangedEvent?.Invoke(typeof(CombatStateStatus), _karma);
                    AchievementCalls.KarmaChanged(_karma);
                }
            }
        }

        public static int MinKarma
        {
            get => _minKarma;
            set
            {
                if (_minKarma != value)
                {
                    _minKarma = value;
                    MinKarmaChangedEvent?.Invoke(typeof(CombatStateStatus), _minKarma);
                }
            }
        }

        public static int MaxKarma
        {
            get => _maxKarma;
            set
            {
                if (_maxKarma != value)
                {
                    _maxKarma = value;
                    MaxKarmaChangedEvent?.Invoke(typeof(CombatStateStatus), _maxKarma);
                }
            }
        }

        public static int Money
        {
            get => _money;
            set
            {
                if (_money != value)
                {
                    if (value > _money)  AchievementCalls.CoinCollected(value, value - _money);
                    _money = value;
                    MoneyChangedEvent?.Invoke(typeof(CombatStateStatus), _money);
                }
            }
        }

        public static int TotalCoins
        {
            get => _totalCoins;
            set
            {
                if (_totalCoins != value)
                {
                    _totalCoins = value;
                }
            }
        }

        public static int GamePoints
        {
            get => _gamePoints;
            set
            {
                if (_gamePoints != value)
                {
                    _gamePoints = value;
                    GamePointsChangedEvent?.Invoke(typeof(CombatStateStatus), _gamePoints);
                    AchievementCalls.PointChanged(_gamePoints);
                }
            }
        }

        public static bool PhaseStarted
        {
            get => _phaseStarted;
            set => _phaseStarted = value;
        }

        public static void IncrementRound()
        {
            Round++;
            AchievementCalls.RoundNext(Round, Karma);          
        }

        public static void AdvancePhase()
        {
            CurrentPhase = _currentPhase switch
            {
                CombatPhase.Start => CombatPhase.Neutral,
                CombatPhase.Neutral => CombatPhase.Past,
                CombatPhase.Past => CombatPhase.Present,
                CombatPhase.Present => CombatPhase.Future,
                CombatPhase.Future => CombatPhase.Neutral,
                _ => CombatPhase.Neutral
            };
        }
    }

    public enum CombatPhase
    {
        Start,
        Neutral,
        Past,
        Present,
        Future
    }
}
