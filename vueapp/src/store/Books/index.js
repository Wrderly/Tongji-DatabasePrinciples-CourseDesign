import { searchBook } from '@/api'
import Vue from 'vue'
const state = {
   booksList:[],
}

const actions = {
    initBooksList({ commit }) {
        let data = {
            searchStr:""
        }
        searchBook(data).then(res=>{
            console.log(res);
            
            if(res.status == 200)
                commit('INITBOOKSLIST',res.books)
        },err=>console.log(err.message))
    }
}

const mutations = {
  
    INITBOOKSLIST(state, data) {
        data = data || [];
        state.booksList = data;
        console.log(state.booksList);
        /*
        data = data || []
        state.booksList = data.filter(item=>{
            return item.status == 1
        })
        */
    }


}

const getters = {

}

export default {
    state,
    actions,
    mutations,
    getters
}