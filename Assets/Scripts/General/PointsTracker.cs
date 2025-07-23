using System;

namespace F
{

	public static class PointsTracker
	{

		public static int pts
		{
			get
			{
				return PointsTracker._pts;
			}
			set
			{
				PointsTracker._pts = value;
				EventHandler<int> pointsChangedEvent = PointsTracker.PointsChangedEvent;
				if (pointsChangedEvent == null)
				{
					return;
				}
				pointsChangedEvent(typeof(PointsTracker), PointsTracker._pts);
			}
		}

		public static EventHandler<int> PointsChangedEvent;

		private static int _pts;
	}
}
