import Vue from 'vue'
import Vuex from 'vuex'
import createPersistedState from 'vuex-persistedstate';

Vue.use(Vuex)

import Books from './Books'
import User from './User'
import Comments from './Comments'
import Borrows from './Borrows'
import Reserve from './Reserve'
import Report from './Report'
import Suppliers from './Suppliers'
import BuyBooks from './BuyBooks'
import BookTypes from './BookTypes'

export default new Vuex.Store({
    modules:{
        Books,
        User,
        Comments,
        Borrows,
        Reserve,
        Report,
        Suppliers,
        BuyBooks,
        BookTypes
    },
    plugins: [
        createPersistedState({
            storage: window.sessionStorage,
            paths: ["Books", "User", "Borrows", "Reserve"]
        })
    ]
})