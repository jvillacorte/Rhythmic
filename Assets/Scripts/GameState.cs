using System;

    public static class GameState
{
        //passes around method confirming if enemy has died or not
        //was going to be for a gate type interaction
        public static bool enemyKilled = false;
        public delegate void EnemyKilledAction();
        public static event EnemyKilledAction OnEnemyKilled;

        public static void EnemyHasDied()
        {
            enemyKilled = true;
            OnEnemyKilled?.Invoke();
        }
    }