import React, { useState, useEffect } from 'react';
import { Line } from 'react-chartjs-2';
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend } from 'chart.js';
import resultService from '../../../services/resultService';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend);

interface ProgressChartProps {
    quizId: number;
}

interface ProgressData {
    attemptNum: number;
    percentage: number;
}

const ProgressChart: React.FC<ProgressChartProps> = ({ quizId }) => {
    const [progressData, setProgressData] = useState<ProgressData[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!quizId) return;
        resultService.getQuizProgress(quizId)
            .then(response => {
                setProgressData(response.data);
            })
            .finally(() => {
                setLoading(false);
            });
    }, [quizId]);
    
    if (loading || progressData.length <= 1) {
        return null; 
    }

    const chartData = {
        labels: progressData.map(p => `Pokušaj #${p.attemptNum}`),
        datasets: [
            {
                label: 'Uspešnost (%)',
                data: progressData.map(p => p.percentage),
                borderColor: '#EED7C5', // var(--text-color)
                backgroundColor: 'rgba(238, 213, 197, 0.5)',
                tension: 0.1
            },
        ],
    };

    const options = {
        responsive: true,
        plugins: {
            legend: { display: false },
            title: {
                display: true,
                text: 'Napredak kroz pokušaje',
                color: '#EED7C5'
            },
        },
        scales: {
            y: {
                beginAtZero: true,
                max: 100,
                ticks: { color: '#EED7C5' }
            },
            x: {
                ticks: { color: '#EED7C5' }
            }
        }
    };

    return (
        <div className="progress-chart-container">
            <Line options={options} data={chartData} />
        </div>
    );
};

export default ProgressChart;