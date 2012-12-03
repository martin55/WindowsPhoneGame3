namespace SharkGameLib
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a single high score entry in the Shark Game.
    /// Serializable, for use with IsolatedStorage.
    /// </summary>
    [DataContract]
    public class HighScore
    {
        /* Fields */

        /// <summary>
        /// Name for this high score entry.
        /// </summary>
        private string name;

        /// <summary>
        /// Timestamp for this high score entry.
        /// </summary>
        private DateTime timestamp;

        /// <summary>
        /// Points for this high score entry.
        /// </summary>
        private int points;

        /* Constructor*/

        /// <summary>
        /// Initializes a new instance of the <see cref="HighScore" /> class.
        /// </summary>
        /// <param name="name">Name for the new high score entry.</param>
        /// <param name="timestamp">Timestamp for the new high score entry.</param>
        /// <param name="points">Points for the new high score entry.</param>
        public HighScore(string name, DateTime timestamp, int points)
        {
            this.name = name;
            this.timestamp = timestamp;
            this.points = points;
        }

        /* Properties */

        /// <summary>
        /// Gets the name for this high score entry.
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets the timestamp for this high score entry.
        /// </summary>
        [DataMember]
        public DateTime Timestamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        /// <summary>
        /// Gets the formatted date for this high score entry.
        /// </summary>
        public string Date
        {
            get { return this.timestamp.ToShortDateString(); }
        }

        /// <summary>
        /// Gets the formatted time for this high score entry.
        /// </summary>
        public string Time
        {
            get { return this.timestamp.ToShortTimeString(); }
        }

        /// <summary>
        /// Gets the points for this high score entry.
        /// </summary>
        [DataMember]
        public int Points
        {
            get { return this.points; }
            set { this.points = value; }
        }
    }
}
