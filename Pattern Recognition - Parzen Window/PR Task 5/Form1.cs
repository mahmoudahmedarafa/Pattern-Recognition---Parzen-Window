using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PR_Task_5
{
    public partial class Form1 : Form
    {
        private int class1_Training, class2_Training, class3_Training;
        private int class1_Testing, class2_Testing, class3_Testing;
        private int windowSize;

        private double[] posterior, likelidhood, prior;
        private double evidence;

        private int[,] confusionMatrix;

        private List <IRIS_flower> class1_Dataset, class2_Dataset, class3_Dataset;
        private List <IRIS_flower> class1_Trainingset, class2_Trainingset, class3_Trainingset;
        private List<IRIS_flower> class1_Testingset, class2_Testingset, class3_Testingset;

        private List<string> decisions;

        public Form1()
        {
            InitializeComponent();
        }

        public void calculateOverallAccuracy()
        {
            int sumDiagonal = 0;
            for (int i = 1; i < 4; i++)
            {
                sumDiagonal += confusionMatrix[i, i];
            }

            int sumTesting = class1_Testing + class2_Testing + class3_Testing;
            double overallAccuracy = (double)sumDiagonal / sumTesting;
            overallAccuracy *= 100;
            overallAccuracy = Math.Round(overallAccuracy, 2);
            label6.Text = overallAccuracy.ToString() + "%";
        }

        public void displayConfusionMatrix()
        {
            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    confusionMatrix[i, 4] += confusionMatrix[i, j];
                }
            }

            for (int i = 1; i < 5; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    confusionMatrix[4, i] += confusionMatrix[j, i];
                }
            }

            for (int i = 1; i < 5; i++)
            {
                var row = new DataGridViewRow();
                for (int j = 1; j < 5; j++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = confusionMatrix[i, j]
                    });
                }
                dataGridView1.Rows.Add(row);
            }
        }

        public void displayDecisions()
        {
            listBox1.DataSource = decisions;
        }

        public void calculatePosterior()
        {
            posterior = new double[4];
            for (int i = 1; i <= 3; i++)
            {
                posterior[i] = (likelidhood[i] * prior[i]) / evidence;
            }
        }

        public void calculateEvidence()
        {
            evidence = 0;
            for (int i = 1; i <= 3; i++)
            {
                evidence += likelidhood[i] * prior[i];
            }
        }

        public double calculateLikelihood(int k, int n, int v)
        {
            return (((double)k / n) / v);
        }

        public bool parzenWindow(IRIS_flower sample, IRIS_flower training)
        {
            double distance = 0;
            for (int i = 0; i < 4; i++)
            {
                distance += Math.Pow(sample.features[i] - training.features[i], 2);
            }
            distance = Math.Sqrt(distance) / windowSize;
            return distance <= 0.5;
        }

        public void classify()
        {
            //Class 1 Testingset
            int testNumber = 1;
            foreach (IRIS_flower sample in class1_Testingset)
            {
                int []k = new int[4];
                int n = 0;  //n should be qual to total number of training samples = 90?

                for (int i = 0; i < class1_Trainingset.Count; i++)
                {
                    int res = Convert.ToInt32(parzenWindow(sample, class1_Trainingset[i]));
                    k[1] += res;
                    n += res;
                }

                for (int i = 0; i < class2_Trainingset.Count; i++)
                {
                    int res = Convert.ToInt32(parzenWindow(sample, class2_Trainingset[i]));
                    k[2] += res;
                    n += res;
                }

                for (int i = 0; i < class3_Trainingset.Count; i++)
                {
                    int res = Convert.ToInt32(parzenWindow(sample, class3_Trainingset[i]));
                    k[3] += res;
                    n += res;
                }

                likelidhood = new double[4];
                int v = (int)Math.Pow(windowSize, 4);
                for(int i = 1; i <= 3; i++)
                {
                    likelidhood[i] = calculateLikelihood(k[i], n, v);
                }

                calculateEvidence();
                calculatePosterior();

                double maximumPosterior = 0;
                for (int i = 1; i <= 3; i++)
                {
                    maximumPosterior = Math.Max(maximumPosterior, posterior[i]);
                }
                int sampleClass = 0;
                for (int i = 1; i <= 3; i++)
                {
                    if (maximumPosterior == posterior[i])
                    {
                        sampleClass = i;
                        confusionMatrix[1, sampleClass]++;
                        break;
                    }
                }

                string classification = "Test " + testNumber.ToString();
                classification += " in class 1 belongs to class " + sampleClass.ToString();
                classification += ".\n";
                decisions.Add(classification);
                testNumber++;
            }

            //Class 2 Testingset
            testNumber = 1;
            foreach (IRIS_flower sample in class2_Testingset)
            {
                int[] k = new int[4];
                int n = 0;  //n should be qual to total number of training samples = 90?

                for (int i = 0; i < class1_Trainingset.Count; i++)
                {
                    int res = Convert.ToInt32(parzenWindow(sample, class1_Trainingset[i]));
                    k[1] += res;
                    n += res;
                }

                for (int i = 0; i < class2_Trainingset.Count; i++)
                {
                    int res = Convert.ToInt32(parzenWindow(sample, class2_Trainingset[i]));
                    k[2] += res;
                    n += res;
                }

                for (int i = 0; i < class3_Trainingset.Count; i++)
                {
                    int res = Convert.ToInt32(parzenWindow(sample, class3_Trainingset[i]));
                    k[3] += res;
                    n += res;
                }

                likelidhood = new double[4];
                int v = (int)Math.Pow(windowSize, 4);
                for (int i = 1; i <= 3; i++)
                {
                    likelidhood[i] = calculateLikelihood(k[i], n, v);
                }

                calculateEvidence();
                calculatePosterior();

                double maximumPosterior = 0;
                for (int i = 1; i <= 3; i++)
                {
                    maximumPosterior = Math.Max(maximumPosterior, posterior[i]);
                }
                int sampleClass = 0;
                for (int i = 1; i <= 3; i++)
                {
                    if (maximumPosterior == posterior[i])
                    {
                        sampleClass = i;
                        confusionMatrix[2, sampleClass]++;
                        break;
                    }
                }

                string classification = "Test " + testNumber.ToString() + " in class 2 belongs to class " + sampleClass.ToString() + ".\n";
                decisions.Add(classification);
                testNumber++;
            }

            //Class 3 Testingset
            testNumber = 1;
            foreach (IRIS_flower sample in class3_Testingset)
            {
                int[] k = new int[4];
                int n = 0;  //n should be qual to total number of training samples = 90?

                for (int i = 0; i < class1_Trainingset.Count; i++)
                {
                    int res = Convert.ToInt32(parzenWindow(sample, class1_Trainingset[i]));
                    k[1] += res;
                    n += res;
                }

                for (int i = 0; i < class2_Trainingset.Count; i++)
                {
                    int res = Convert.ToInt32(parzenWindow(sample, class2_Trainingset[i]));
                    k[2] += res;
                    n += res;
                }

                for (int i = 0; i < class3_Trainingset.Count; i++)
                {
                    int res = Convert.ToInt32(parzenWindow(sample, class3_Trainingset[i]));
                    k[3] += res;
                    n += res;
                }

                likelidhood = new double[4];
                int v = (int)Math.Pow(windowSize, 4);
                for (int i = 1; i <= 3; i++)
                {
                    likelidhood[i] = calculateLikelihood(k[i], n, v);
                }

                calculateEvidence();
                calculatePosterior();

                double maximumPosterior = 0;
                for (int i = 1; i <= 3; i++)
                {
                    maximumPosterior = Math.Max(maximumPosterior, posterior[i]);
                }
                int sampleClass = 0;
                for (int i = 1; i <= 3; i++)
                {
                    if (maximumPosterior == posterior[i])
                    {
                        sampleClass = i;
                        confusionMatrix[3, sampleClass]++;
                        break;
                    }
                }

                string classification = "Test " + testNumber.ToString() + " in class 3 belongs to class " + sampleClass.ToString() + ".\n";
                decisions.Add(classification);
                testNumber++;
            }
        }

        public int generateRandomNumber()
        {
            Random r = new Random();
            return r.Next(0, 49);
        }

        public void selectTrainingset()
        {
            int cnt = 0;
            List<bool> isTaken = new List<bool>(new bool[50]);
            for (int i = 0; i < 50; i++)
            {
                isTaken[i] = false;
            }
            while (cnt < class1_Training)
            {
                int index = generateRandomNumber();
                while (isTaken[index])
                {
                    index = generateRandomNumber();
                }
                isTaken[index] = true;
                class1_Trainingset.Add(class1_Dataset[index]);
                cnt++;
            }
            for (int i = 0; i < 50; i++)
            {
                if (isTaken[i] == false)
                    class1_Testingset.Add(class1_Dataset[i]);
            }

            cnt = 0;
            for (int i = 0; i < 50; i++)
            {
                isTaken[i] = false;
            }
            while (cnt < class2_Training)
            {
                int index = generateRandomNumber();
                while (isTaken[index])
                {
                    index = generateRandomNumber();
                }
                isTaken[index] = true;
                class2_Trainingset.Add(class2_Dataset[index]);
                cnt++;
            }
            for (int i = 0; i < 50; i++)
            {
                if (isTaken[i] == false)
                    class2_Testingset.Add(class2_Dataset[i]);
            }

            cnt = 0;
            for (int i = 0; i < 50; i++)
            {
                isTaken[i] = false;
            }
            while (cnt < class3_Training)
            {
                int index = generateRandomNumber();
                while (isTaken[index])
                {
                    index = generateRandomNumber();
                }
                isTaken[index] = true;
                class3_Trainingset.Add(class3_Dataset[index]);
                cnt++;
            }
            for (int i = 0; i < 50; i++)
            {
                if (isTaken[i] == false)
                    class3_Testingset.Add(class3_Dataset[i]);
            }
        }

        public void calculatePrior()
        {
            prior = new double[4];
            int total_Training = class1_Training + class2_Training + class3_Training;

            prior[1] = (double)class1_Training / total_Training;
            prior[2] = (double)class2_Training / total_Training;
            prior[3] = (double)class3_Training / total_Training;
        }

        public void parseInput()
        {
            class1_Training = int.Parse(textBox1.Text);
            class2_Training = int.Parse(textBox2.Text);
            class3_Training = int.Parse(textBox3.Text);
            windowSize = int.Parse(textBox4.Text);
            class1_Testing = 50 - class1_Training;
            class2_Testing = 50 - class2_Training;
            class3_Testing = 50 - class3_Training;
        }

        public void parseFile()
        {
            string[] lines = System.IO.File.ReadAllLines(@"E:\College\Pattern Recognition\Labs\Lab5\Task (4)\Iris Data.txt");
            for (int i = 1; i < lines.Length; i++ )
            {
                string[] tmp = lines[i].Split(',');
                IRIS_flower obj = new IRIS_flower();
                for (int j = 0; j < 4; j++)
                {
                    obj.features[j] = double.Parse(tmp[j]);
                }
                if (tmp[4] == "Iris-setosa")
                    class1_Dataset.Add(obj);
                else if (tmp[4] == "Iris-versicolor")
                    class2_Dataset.Add(obj);
                else if (tmp[4] == "Iris-virginica")
                    class3_Dataset.Add(obj);
            }
        }

        public void initializeSets()
        {
            class1_Dataset = new List<IRIS_flower>();
            class2_Dataset = new List<IRIS_flower>();
            class3_Dataset = new List<IRIS_flower>();

            class1_Trainingset = new List<IRIS_flower>();
            class2_Trainingset = new List<IRIS_flower>();
            class3_Trainingset = new List<IRIS_flower>();

            class1_Testingset = new List<IRIS_flower>();
            class2_Testingset = new List<IRIS_flower>();
            class3_Testingset = new List<IRIS_flower>();

            decisions = new List<string>();

            confusionMatrix = new int[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    confusionMatrix[i, j] = 0;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            parseInput();
            calculatePrior();
            initializeSets();
            parseFile();
            selectTrainingset();
            classify();
            displayDecisions();
            displayConfusionMatrix();
            calculateOverallAccuracy();
        }
    }
}
