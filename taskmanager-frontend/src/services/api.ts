/// <reference types="vite/client" />
import axios, { AxiosInstance } from 'axios';
import { 
  LoginRequest, 
  RegisterRequest, 
  AuthResponse, 
  TaskItem, 
  CreateTaskRequest, 
  UpdateTaskRequest 
} from '../types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001/api';

class ApiClient {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: API_BASE_URL,
      withCredentials: true,
    });

    // Interceptor para adicionar token JWT em todas as requisições
    this.axiosInstance.interceptors.request.use((config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });

    // Interceptor para tratar erros de autenticação
    this.axiosInstance.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('token');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Auth endpoints
  async login(payload: LoginRequest): Promise<AuthResponse> {
    const response = await this.axiosInstance.post<AuthResponse>('/auth/login', payload);
    return response.data;
  }

  async register(payload: RegisterRequest): Promise<AuthResponse> {
    const response = await this.axiosInstance.post<AuthResponse>('/auth/register', payload);
    return response.data;
  }

  // Task endpoints
  async getTasks(): Promise<TaskItem[]> {
    const response = await this.axiosInstance.get<TaskItem[]>('/tasks');
    return response.data;
  }

  async getTask(id: string): Promise<TaskItem> {
    const response = await this.axiosInstance.get<TaskItem>(`/tasks/${id}`);
    return response.data;
  }

  async createTask(payload: CreateTaskRequest): Promise<TaskItem> {
    const response = await this.axiosInstance.post<TaskItem>('/tasks', payload);
    return response.data;
  }

  async updateTask(id: string, payload: UpdateTaskRequest): Promise<TaskItem> {
    const response = await this.axiosInstance.put<TaskItem>(`/tasks/${id}`, payload);
    return response.data;
  }

  async updateTaskStatus(id: string, status: string): Promise<TaskItem> {
    const response = await this.axiosInstance.patch<TaskItem>(`/tasks/${id}`, { status });
    return response.data;
  }

  async deleteTask(id: string): Promise<void> {
    await this.axiosInstance.delete(`/tasks/${id}`);
  }
}

export const apiClient = new ApiClient();
