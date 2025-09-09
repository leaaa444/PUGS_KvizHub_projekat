import React from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from '../../context/AuthContext';

interface AdminRouteProps {
  children: React.ReactNode;
}

const AdminRoute: React.FC<AdminRouteProps> = ({ children }) => {
  const { user, loading } = useAuth();

  if (loading) {
    return <div>Učitavanje...</div>; 
  }

  if (!user || user.role !== 'Admin') {
    return <Navigate to="/kvizovi" replace />;
  }

  return <>{children}</>;


};

export default AdminRoute;
