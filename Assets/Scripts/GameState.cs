using System;

    public static class GameState
    {
        public static bool enemyKilled = false;
        public delegate void EnemyKilledAction();
        public static event EnemyKilledAction OnEnemyKilled;

        public static void EnemyHasDied()
        {
            enemyKilled = true;
            OnEnemyKilled?.Invoke();
        }
    }

