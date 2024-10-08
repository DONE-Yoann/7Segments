using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SevenSegmentDisplayApp
{
    public partial class MainWindow : Window
    {
        private TextBox[] saisieEquations; // Tableau de TextBox pour les saisies des équations de chaque segment
        private Rectangle[] segments; // Tableau de Rectangles représentant chaque segment de l'afficheur 7 segments

        public MainWindow()
        {
            InitializeComponent();
            // Initialise les TextBox pour les équations de chaque segment
            saisieEquations = new TextBox[] { EquationInputA, EquationInputB, EquationInputC, EquationInputD, EquationInputE, EquationInputF, EquationInputG };
            // Initialise les rectangles pour chaque segment de l'afficheur 7 segments
            segments = new Rectangle[] { SegmentA, SegmentB, SegmentC, SegmentD, SegmentE, SegmentF, SegmentG };
        }

        // Gestionnaire d'événement pour le clic sur le bouton Test Input
        private void BoutonTester_Click(object sender, RoutedEventArgs e)
        {
            // Appelle la méthode pour tester l'entrée avec la valeur saisie par l'utilisateur
            TesterEntree(InputBox.Text);
        }

        // Teste la combinaison d'entrée donnée par l'utilisateur
        private void TesterEntree(string entree)
        {
            // Vérifie que la saisie est bien un nombre binaire de 4 bits (par exemple : 0000, 1010, etc.)
            if (entree.Length != 4 || !entree.All(c => c == '0' || c == '1'))
            {
                MessageBox.Show("Entrée invalide. Veuillez entrer un nombre binaire de 4 bits.");
                return;
            }

            // Convertit la saisie en tableau de booléens (true pour '1', false pour '0')
            bool[] bitsEntree = entree.Select(c => c == '1').ToArray();
            for (int i = 0; i < 7; i++)
            {
                // Évalue l'équation saisie par l'utilisateur pour chaque segment
                // Si l'évaluation est vraie, le segment est coloré en rouge, sinon il est gris
                segments[i].Fill = EvaluerEquationSegment(saisieEquations[i].Text, bitsEntree, (char)('A' + i)) ? Brushes.Red : Brushes.Gray;
            }
        }

        // Évalue l'équation booléenne pour déterminer l'état du segment
        private bool EvaluerEquationSegment(string equation, bool[] bitsEntree, char segment)
        {
            // Assigne les bits d'entrée aux variables booléennes A, B, C, D (chaque bit représente une entrée binaire)
            bool A = bitsEntree[3]; // Bit d'entrée le plus significatif
            bool B = bitsEntree[2];
            bool C = bitsEntree[1];
            bool D = bitsEntree[0]; // Bit d'entrée le moins significatif

            // Remplace les variables a, b, c, d dans l'équation par leurs valeurs booléennes respectives
            // "a/" représente la négation de 'a', remplacé par "tr" (True) ou "fl" (False)
            string replacedEquation = equation.Replace("a/", (!A ? "tr" : "fl"))
                                              .Replace("a", (A ? "tr" : "fl"))
                                              .Replace("b/", (!B ? "tr" : "fl"))
                                              .Replace("b", (B ? "tr" : "fl"))
                                              .Replace("c/", (!C ? "tr" : "fl"))
                                              .Replace("c", (C ? "tr" : "fl"))
                                              .Replace("d/", (!D ? "tr" : "fl"))
                                              .Replace("d", (D ? "tr" : "fl"))
                                              .Replace(" ", ""); // Supprime les espaces inutiles

            try
            {
                // Remplace les opérateurs de l'équation pour les rendre compatibles avec l'évaluation booléenne
                replacedEquation = replacedEquation.Replace("+", " OR ").Replace(".", " AND ");
                replacedEquation = replacedEquation.Replace("tr", "True").Replace("fl", "False");

                // Utilise DataTable pour évaluer l'expression booléenne finale
                var table = new System.Data.DataTable();
                bool resultat = (bool)table.Compute(replacedEquation, "");
                return resultat; // Retourne le résultat de l'évaluation de l'équation
            }
            catch (Exception ex)
            {
                // Affiche un message d'erreur si l'équation est invalide
                MessageBox.Show($"Format d'équation invalide pour le segment {segment} : {ex.Message}. Veuillez corriger l'équation.");
                return false;
            }
        }
    }
}
