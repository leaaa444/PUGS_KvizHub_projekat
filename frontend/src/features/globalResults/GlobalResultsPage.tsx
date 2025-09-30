import React, { useState,useMemo } from 'react';
import GlobalRankingTab from './components/GlobalRankingTab';
import PerQuizRankingsTab from './components/PerQuizRankingsTab';
import LiveGlobalRankingTab from './components/LiveGlobalRankingTab';
import ArenaHistoryTab from './components/ArenaHistoryTab';
import './GlobalResultsPage.css';

type Tab = 'globalStandard' | 'globalLive' | 'perQuiz' | 'arenaHistory';
export type TimePeriod = 'week' | 'month' | 'all';

const formatDate = (date: Date) => {
    return date.toISOString().split('T')[0];
};


const GlobalResultsPage: React.FC = () => {
    const [activeTab, setActiveTab] = useState<Tab>('globalStandard');
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
            startDate: timePeriod !== 'all' ? formatDate(startDate) : undefined,
            endDate: timePeriod !== 'all' ? formatDate(endDate) : undefined
        };
    }, [timePeriod]);

    const renderActiveTab = () => {
        switch(activeTab) {
            case 'globalStandard':
                return <GlobalRankingTab startDate={dates.startDate} endDate={dates.endDate} />;
            case 'globalLive':
                return <LiveGlobalRankingTab startDate={dates.startDate} endDate={dates.endDate} />;
            case 'perQuiz':
                return <PerQuizRankingsTab startDate={dates.startDate} endDate={dates.endDate} />;
            case 'arenaHistory':
                return <ArenaHistoryTab />;
            default:
                return null;
        }
    }

  return (
        <div className="ranking-page-container">
            <h1>Rang Lista</h1>
            {activeTab !== 'arenaHistory' && (
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
            )}
            
            <div className="tabs-container">
                <button 
                    className={`tab-button ${activeTab === 'globalStandard' ? 'active' : ''}`}
                    onClick={() => setActiveTab('globalStandard')}
                >
                    Globalna Rang Lista
                </button>
                <button 
                    className={`tab-button ${activeTab === 'globalLive' ? 'active' : ''}`}
                    onClick={() => setActiveTab('globalLive')}
                >
                    Globalni Rang (Live)
                </button>
                <button 
                    className={`tab-button ${activeTab === 'perQuiz' ? 'active' : ''}`}
                    onClick={() => setActiveTab('perQuiz')}
                >
                    Rezultati po Kvizovima
                </button>
                <button 
                    className={`tab-button ${activeTab === 'arenaHistory' ? 'active' : ''}`}
                    onClick={() => setActiveTab('arenaHistory')}
                >
                    Istorija Arena
                </button>
            </div>

            <div className="tab-content">
                {renderActiveTab()}
            </div>
        </div>
    );
};

export default GlobalResultsPage;