namespace AStarAlgorithm;

public partial class AlogorithmDisplay : Form
{
    public AlogorithmDisplay()
    {
        InitializeComponent();
        this.BackColor = System.Drawing.Color.Black;
        this.DoubleBuffered = true;
        AlgorithmLogic algorithm = new AlgorithmLogic(this);
    }
}
