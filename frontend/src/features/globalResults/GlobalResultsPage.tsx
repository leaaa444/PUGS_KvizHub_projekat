import React, { useState } from 'react';
import GlobalRankingTab from './components/GlobalRankingTab';
import PerQuizRankingsTab from './components/PerQuizRankingsTab';
import './GlobalResultsPage.css';

type Tab = 'global' | 'perQuiz';


const GlobalResultsPage: React.FC = () => {
    const [activeTab, setActiveTab] = useState<Tab>('global');

  return (
        <div className="ranking-page-container">
            <h1>Rang Lista</h1>
            
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
                {activeTab === 'global' ? <GlobalRankingTab /> : <PerQuizRankingsTab />}
            </div>
        </div>
    );
};

export default GlobalResultsPage;