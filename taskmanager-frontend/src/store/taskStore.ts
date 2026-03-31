import { create } from 'zustand';
import { TaskItem, TaskStatus } from '../types';
import { apiClient } from '../services/api';

interface TaskStore {
  tasks: TaskItem[];
  loading: boolean;
  error: string | null;
  
  // Actions
  fetchTasks: () => Promise<void>;
  addTask: (title: string, description?: string) => Promise<void>;
  updateTask: (id: string, title: string, description?: string) => Promise<void>;
  updateTaskStatus: (id: string, status: TaskStatus) => Promise<void>;
  deleteTask: (id: string) => Promise<void>;
  setError: (error: string | null) => void;
}

export const useTaskStore = create<TaskStore>((set) => ({
  tasks: [],
  loading: false,
  error: null,

  fetchTasks: async () => {
    set({ loading: true, error: null });
    try {
      const tasks = await apiClient.getTasks();
      set({ tasks, loading: false });
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Erro ao carregar tarefas';
      set({ error: errorMessage, loading: false });
    }
  },

  addTask: async (title: string, description?: string) => {
    set({ loading: true, error: null });
    try {
      const newTask = await apiClient.createTask({ title, description, status: TaskStatus.Pending });
      set((state) => ({
        tasks: [...state.tasks, newTask],
        loading: false,
      }));
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Erro ao criar tarefa';
      set({ error: errorMessage, loading: false });
    }
  },

  updateTask: async (id: string, title: string, description?: string) => {
    set({ loading: true, error: null });
    try {
      const updated = await apiClient.updateTask(id, { title, description });
      set((state) => ({
        tasks: state.tasks.map((t) => (t.id === id ? updated : t)),
        loading: false,
      }));
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Erro ao atualizar tarefa';
      set({ error: errorMessage, loading: false });
    }
  },

  updateTaskStatus: async (id: string, status: TaskStatus) => {
    set({ loading: true, error: null });
    try {
      const updated = await apiClient.updateTaskStatus(id, status);
      set((state) => ({
        tasks: state.tasks.map((t) => (t.id === id ? updated : t)),
        loading: false,
      }));
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Erro ao atualizar status';
      set({ error: errorMessage, loading: false });
    }
  },

  deleteTask: async (id: string) => {
    set({ loading: true, error: null });
    try {
      await apiClient.deleteTask(id);
      set((state) => ({
        tasks: state.tasks.filter((t) => t.id !== id),
        loading: false,
      }));
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Erro ao deletar tarefa';
      set({ error: errorMessage, loading: false });
    }
  },

  setError: (error: string | null) => set({ error }),
}));
