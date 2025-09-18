import React, { useState,useMemo } from 'react';
import GlobalRankingTab from './components/GlobalRankingTab';
import PerQuizRankingsTab from './components/PerQuizRankingsTab';
import './GlobalResultsPage.css';

type Tab = 'global' | 'perQuiz';
export type TimePeriod = 'week' | 'month' | 'all';

const formatDate = (date: Date) => {
    return date.toISOString().split('T')[0];
};


const GlobalResultsPage: React.FC = () => {
    const [activeTab, setActiveTab] = useState<Tab>('global');
    const [timePeriod, setTimePeriod] = useState<TimePeriod>('all');

    const dates = useMemo(() => {
        const endDate = new Date();
        let startDate = new Date();

        if (timePeriod === 'week') {
            startDate.setDate(endDate.getDate() - 7);
        } else if (timePeriod === 'month') {
            startDate.setMonth(endDate.getMonth() - 1);
        }

        return {
            // Vraćamo null ako je izabrano "Sve vreme"
            startDate: timePeriod !== 'all' ? formatDate(startDate) : undefined,
            endDate: timePeriod !== 'all' ? formatDate(endDate) : undefined
        };
    }, [timePeriod]);

  return (
        <div className="ranking-page-container">
            <h1>Rang Lista</h1>

            <div className="time-period-filter">
                <button 
                    className={timePeriod === 'week' ? 'active' : ''}
                    onClick={() => setTimePeriod('week')}
                >
                    Poslednjih 7 dana
                </button>
                <button 
                    className={timePeriod === 'month' ? 'active' : ''}
                    onClick={() => setTimePeriod('month')}
                >
                    Poslednjih 30 dana
                </button>
                <button 
                    className={timePeriod === 'all' ? 'active' : ''}
                    onClick={() => setTimePeriod('all')}
                >
                    Sve vreme
                </button>
            </div>
            
            <div className="tabs-container">
                <button 
                    className={`tab-button ${activeTab === 'global' ? 'active' : ''}`}
                    onClick={() => setActiveTab('global')}
                >
                    Globalna Rang Lista
                </button>
                <button 
                    className={`tab-button ${activeTab === 'perQuiz' ? 'active' : ''}`}
                    onClick={() => setActiveTab('perQuiz')}
                >
                    Rezultati po Kvizovima
                </button>
            </div>

            <div className="tab-content">
                {activeTab === 'global' ? 
                    <GlobalRankingTab startDate={dates.startDate} endDate={dates.endDate} /> : 
                    <PerQuizRankingsTab startDate={dates.startDate} endDate={dates.endDate} />
                }
            </div>
        </div>
    );
};

export default GlobalResultsPage;