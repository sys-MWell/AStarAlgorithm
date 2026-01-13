namespace AStarAlgorithm
{
    /// <summary>
    /// Main application window hosting the A* visualization.
    /// </summary>
    public partial class AlogorithmDisplay : Form
    {
        /// <summary>
        /// Initializes the form and starts the A* algorithm visualization.
        /// </summary>
        public AlogorithmDisplay()
        {
            InitializeComponent();
            this.BackColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true;
            AlgorithmLogic algorithm = new AlgorithmLogic(this);
        }
    }
}
